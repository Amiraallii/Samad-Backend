using Microsoft.EntityFrameworkCore;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;
using Samad.Infrastructure.IRepositories;

namespace Samad.Application.Services
{
    public sealed class CouncilReviewService(
    IRepository<Request, int> requestRepository,
    IRepository<CouncilReview, int> reviewRepository,
    IRepository<RequestCouncilAssignment, int> assignmentRepository,
    IRequestWorkflowService workflowService,
    IUnitOfWork unitOfWork,
    AppSettings settings)
        : ICouncilReviewService
    {
        public async Task<List<CouncilRequestDto>> GetAssignedRequests(
            int councilMemberId,
            CancellationToken cancellationToken = default)
        {
            return await assignmentRepository
                .Query()
                .Where(x =>
                    x.CouncilMemberId == councilMemberId &&
                    x.AssignmentType ==
                    CouncilAssignmentType.Reviewer)
                .Where(x =>
                    x.Request.Status ==
                    RequestStatus.UnderCouncilReview)
                .OrderByDescending(x => x.Request.CreatedAt)
                .Select(x =>
                    new CouncilRequestDto(
                        x.Request.Id,
                        x.Request.Title,
                        x.Request.Description,
                        x.Request.Urgency,
                        x.Request.Status,
                        x.Request.CreatedAt,
                        x.Request.DeadlineAt,
                        x.Request.CouncilReviews.Any(
                            r =>
                                r.CouncilMemberId ==
                                councilMemberId)))
                .ToListAsync(cancellationToken);
        }

        public async Task<CouncilRequestDetailsDto> GetRequest(
            int requestId,
            int councilMemberId,
            CancellationToken cancellationToken = default)
        {
            var request =
                await requestRepository
                    .Query()
                    .Include(x => x.Applicant)
                    .Include(x => x.Documents)
                    .Include(x => x.CouncilAssignments)
                    .Include(x => x.CouncilReviews)
                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            if (request.Status !=
                RequestStatus.UnderCouncilReview)
            {
                throw new InvalidOperationException(
                    "این درخواست در مرحله بررسی شورا نیست.");
            }

            var assignment =
                request.CouncilAssignments.FirstOrDefault(x =>
                    x.CouncilMemberId == councilMemberId &&
                    x.AssignmentType ==
                    CouncilAssignmentType.Reviewer);

            if (assignment is null)
            {
                throw new UnauthorizedAccessException(
                    "این درخواست به شما تخصیص داده نشده است.");
            }

            var hasReviewed =
                request.CouncilReviews.Any(
                    x =>
                        x.CouncilMemberId ==
                        councilMemberId);

            var documents =
                request.Documents
                    .Select(x =>
                        new CouncilRequestDocumentDto(
                            x.Id,
                            BuildFileUrl(x.FileUrl),
                            x.ContentType,
                            x.DocumentType))
                    .ToList();

            return new CouncilRequestDetailsDto(
    request.Id,
    request.Title,
    request.Description,
    request.Urgency,
    request.Status,
    request.CreatedAt,
    request.DeadlineAt,
    $"{request.Applicant.FirstName} " +
    $"{request.Applicant.LastName}",
    documents,
    hasReviewed);
        }

        public async Task SubmitReview(
            int requestId,
            int councilMemberId,
            SubmitCouncilReviewDto dto,
            CancellationToken cancellationToken = default)
        {
            ValidateVote(dto.Vote);

            var request =
                await requestRepository
                    .QueryTracking()
                    .Include(x => x.CouncilAssignments)
                    .Include(x => x.CouncilReviews)
                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            if (request.Status !=
                RequestStatus.UnderCouncilReview)
            {
                throw new InvalidOperationException(
                    "این درخواست در مرحله بررسی شورا نیست.");
            }

            var assignment =
                request.CouncilAssignments.FirstOrDefault(x =>
                    x.CouncilMemberId == councilMemberId &&
                    x.AssignmentType ==
                    CouncilAssignmentType.Reviewer);

            if (assignment is null)
            {
                throw new UnauthorizedAccessException(
                    "این درخواست به شما تخصیص داده نشده است.");
            }

            var alreadyReviewed =
                request.CouncilReviews.Any(
                    x =>
                        x.CouncilMemberId ==
                        councilMemberId);

            if (alreadyReviewed)
            {
                throw new InvalidOperationException(
                    "شما قبلاً برای این درخواست نظر ثبت کرده‌اید.");
            }

            var review =
                new CouncilReview
                {
                    RequestId = requestId,
                    CouncilMemberId = councilMemberId,
                    Vote = dto.Vote,
                    Comment = dto.Comment?.Trim(),
                    ReviewDate = DateTime.UtcNow
                };

            await reviewRepository.AddAsync(review);

            await unitOfWork.SaveChangesAsync();

            await UpdateRequestStatusIfCouncilFinished(
                requestId,
                cancellationToken);
        }

        private async Task UpdateRequestStatusIfCouncilFinished(
            int requestId,
            CancellationToken cancellationToken)
        {
            var requiredVotes =
                await assignmentRepository
                    .Query()
                    .CountAsync(
                        x =>
                            x.RequestId == requestId &&
                            x.AssignmentType ==
                            CouncilAssignmentType.Reviewer,
                        cancellationToken);

            if (requiredVotes == 0)
            {
                return;
            }

            var submittedVotes =
    await reviewRepository
        .Query()
        .CountAsync(
            review =>
                review.RequestId == requestId &&
                assignmentRepository
                    .Query()
                    .Any(assignment =>
                        assignment.RequestId == requestId &&
                        assignment.CouncilMemberId ==
                            review.CouncilMemberId &&
                        assignment.AssignmentType ==
                            CouncilAssignmentType.Reviewer),
            cancellationToken);

            if (submittedVotes < requiredVotes)
            {
                return;
            }

            var request =
                await requestRepository
                    .QueryTracking()
                    .SingleAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request.Status !=
                RequestStatus.UnderCouncilReview)
            {
                return;
            }

            await workflowService.ChangeStatus(
    request,
    RequestStatus.AwaitingSecretaryFinalReview,
    null,
    "تمام اعضای شورای بررسی‌کننده نظر خود را ثبت کردند.");

            await unitOfWork.SaveChangesAsync();
        }

        private static void ValidateVote(
            CouncilVote vote)
        {
            if (!Enum.IsDefined(
                    typeof(CouncilVote),
                    vote))
            {
                throw new ArgumentException(
                    "نوع رأی معتبر نیست.",
                    nameof(vote));
            }
        }

        private string BuildFileUrl(
            string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return string.Empty;
            }

            return
                $"{settings.S3Storage.BaseUrlWithBucket.TrimEnd('/')}/" +
                $"{fileUrl.TrimStart('/')}";
        }
    }
}