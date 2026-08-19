using Microsoft.EntityFrameworkCore;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;
using Samad.Infrastructure.IRepositories;

namespace Samad.Application.Services
{
    public sealed class SecretaryRequestService(
        IRepository<Request, int> requestRepository,
        IUnitOfWork unitOfWork,
        AppSettings settings)
        : ISecretaryRequestService
    {
        public async Task<List<SecretaryRequestDto>> GetPendingRequests(
            CancellationToken cancellationToken = default)
        {
            var requests =
                await requestRepository
                    .Query()
                    .Where(x =>
                        x.Status ==
                        RequestStatus.AwaitingSecretary)
                    .OrderByDescending(x =>
                        x.CreatedAt)
                    .Select(x =>
                        new SecretaryRequestDto(
                            x.Id,
                            x.Title,
                            x.Description,
                            x.Urgency,
                            x.Status,
                            x.CreatedAt,

                            x.Applicant.FirstName
                            + " "
                            + x.Applicant.LastName,

                            x.CouncilAssignments.Count(),

                            x.CouncilReviews.Count(r =>
                                r.Vote ==
                                CouncilVote.Approved),

                            x.CouncilReviews.Count(r =>
                                r.Vote ==
                                CouncilVote.Rejected),

                            x.CouncilReviews.Count(r =>
                                r.Vote ==
                                CouncilVote.NeedsRevision)))
                    .ToListAsync(
                        cancellationToken);

            return requests;
        }

        public async Task<SecretaryRequestDetailsDto> GetRequest(
            int requestId,
            CancellationToken cancellationToken = default)
        {
            var request =
                await requestRepository
                    .Query()
                    .Include(x => x.Applicant)
                    .Include(x => x.Documents)
                    .Include(x => x.CouncilReviews)
                        .ThenInclude(x =>
                            x.CouncilMember)
                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            if (request.Status !=
                RequestStatus.AwaitingSecretary)
            {
                throw new InvalidOperationException(
                    "این درخواست هنوز آماده بررسی نهایی دبیر نیست.");
            }

            var applicant =
                new SecretaryApplicantDto(
                    request.Applicant.Id,
                    request.Applicant.FirstName,
                    request.Applicant.LastName,
                    request.Applicant.NationalCode,
                    request.Applicant.Email,
                    request.Applicant.PhoneNumber);

            var documents =
                request.Documents
                    .Select(x =>
                        new SecretaryDocumentDto(
                            x.Id,
                            BuildFileUrl(x.FileUrl),
                            x.ContentType))
                    .ToList();

            var reviews =
                request.CouncilReviews
                    .OrderBy(x =>
                        x.ReviewDate)
                    .Select(x =>
                        new SecretaryCouncilReviewDto(
                            x.CouncilMemberId,

                            x.CouncilMember.FirstName
                            + " "
                            + x.CouncilMember.LastName,

                            x.Vote,
                            x.Comment,
                            x.ReviewDate))
                    .ToList();

            return new SecretaryRequestDetailsDto(
                request.Id,
                request.Title,
                request.Description,
                request.Urgency,
                request.Status,
                request.CreatedAt,
                applicant,
                documents,
                reviews);
        }

        public async Task SubmitDecision(
            int requestId,
            int secretaryId,
            SecretaryDecisionDto dto,
            CancellationToken cancellationToken = default)
        {
            var request =
                await requestRepository
                    .QueryTracking()
                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            if (request.Status !=
                RequestStatus.AwaitingSecretary)
            {
                throw new InvalidOperationException(
                    "این درخواست در مرحله تصمیم نهایی دبیر نیست.");
            }

            request.SecretaryId =
                secretaryId;

            request.FinalSecretaryComment =
                dto.Comment.Trim();

            request.FinalDecisionDate =
                DateTime.UtcNow;

            request.Status =
                dto.Decision switch
                {
                    SecretaryDecision.Approved =>
                        RequestStatus.Approved,

                    SecretaryDecision.Rejected =>
                        RequestStatus.Rejected,

                    SecretaryDecision.NeedsRevision =>
                        RequestStatus.NeedsRevision,

                    _ => throw new InvalidOperationException(
                        "تصمیم دبیر معتبر نیست.")
                };

            await unitOfWork.SaveChangesAsync();
        }

        private string BuildFileUrl(
            string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return string.Empty;

            return
                $"{settings.S3Storage.BaseUrlWithBucket.TrimEnd('/')}/{fileUrl.TrimStart('/')}";
        }
    }
}