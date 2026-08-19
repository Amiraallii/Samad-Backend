using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;
using Samad.Infrastructure.IRepositories;
namespace Samad.Application.Services
{
    public sealed class CouncilReviewService(
        IRepository<Domain.Entity.Request, int> requestRepository,
        IRepository<CouncilReview, int> reviewRepository,
        IRepository<RequestCouncilAssignment, int> assignmentRepository,
        IUnitOfWork unitOfWork,
        AppSettings settings)
        : ICouncilReviewService
    {
        public async Task<List<CouncilRequestDto>> GetAssignedRequests(
            int councilMemberId,
            CancellationToken cancellationToken = default)
        {
            var result =
                await assignmentRepository
                    .Query()
                    .Where(x =>
                        x.CouncilMemberId == councilMemberId)
                    .Where(x =>
                        x.Request.Status == RequestStatus.UnderCouncilReview ||
                        x.Request.Status == RequestStatus.UnderCouncilReview)
                    .OrderByDescending(x =>
                        x.Request.CreatedAt)
                    .Select(x =>
                        new CouncilRequestDto(
                            x.Request.Id,
                            x.Request.Title,
                            x.Request.Description,
                            x.Request.Urgency,
                            x.Request.Status,
                            x.Request.CreatedAt,

                            x.Request.CouncilReviews.Any(r =>
                                r.CouncilMemberId ==
                                councilMemberId)))
                    .ToListAsync(cancellationToken);

            return result;
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

            var isAssigned =
                request.CouncilAssignments.Any(
                    x =>
                        x.CouncilMemberId ==
                        councilMemberId);

            if (!isAssigned)
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
                            x.ContentType))
                    .ToList();

            return new CouncilRequestDetailsDto(
                request.Id,
                request.Title,
                request.Description,
                request.Urgency,
                request.Status,
                request.CreatedAt,

                $"{request.Applicant.FirstName} {request.Applicant.LastName}",

                documents,

                hasReviewed);
        }

        public async Task SubmitReview(
            int requestId,
            int councilMemberId,
            SubmitCouncilReviewDto dto,
            CancellationToken cancellationToken = default)
        {
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

            var isAssigned =
                request.CouncilAssignments.Any(
                    x =>
                        x.CouncilMemberId ==
                        councilMemberId);

            if (!isAssigned)
            {
                throw new UnauthorizedAccessException(
                    "این درخواست به شما تخصیص داده نشده است.");
            }

            if (request.Status is not
                RequestStatus.UnderCouncilReview and not
                RequestStatus.UnderCouncilReview)
            {
                throw new InvalidOperationException(
                    "این درخواست در مرحله بررسی شورا نیست.");
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

            var review = new CouncilReview
            {
                RequestId = requestId,

                CouncilMemberId =
                    councilMemberId,

                Vote = dto.Vote,

                Comment =
                    dto.Comment.Trim(),

                ReviewDate =
                    DateTime.UtcNow
            };

            await reviewRepository.AddAsync(review);

            request.Status =
                RequestStatus.UnderCouncilReview;

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
                        x => x.RequestId == requestId,
                        cancellationToken);

            var submittedVotes =
                await reviewRepository
                    .Query()
                    .CountAsync(
                        x => x.RequestId == requestId,
                        cancellationToken);

            if (requiredVotes == 0)
                return;

            if (submittedVotes < requiredVotes)
                return;

            var request =
                await requestRepository
                    .QueryTracking()
                    .SingleAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            request.Status =
                RequestStatus.AwaitingSecretary;

            await unitOfWork.SaveChangesAsync();
        }

        private string BuildFileUrl(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return string.Empty;

            return
                $"{settings.S3Storage.BaseUrlWithBucket.TrimEnd('/')}/{fileUrl.TrimStart('/')}";
        }
    }
}
