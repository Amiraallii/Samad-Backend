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
    IRepository<User, int> userRepository,
    IRepository<CouncilMember, int> councilMemberRepository,
    IRepository<RequestCouncilAssignment, int> assignmentRepository,
    IRepository<CouncilSignature, int> councilSignatureRepository,
    IRepository<SecretaryDecision, int> secretaryDecisionRepository,
    IRequestWorkflowService workflowService,
    IUnitOfWork unitOfWork,
    AppSettings settings)
    : ISecretaryRequestService
    {
        public async Task<List<SecretaryRequestDto>> GetInitialReviewRequests(
    CancellationToken cancellationToken = default)
        {
            var requests =
                await requestRepository
                    .Query()
                    .Where(x =>
                        x.Status ==
                        RequestStatus.AwaitingSecretaryInitialReview)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x =>
                        new SecretaryRequestProjection(
                            x.Id,
                            x.Title,
                            x.Description,
                            x.Urgency,
                            x.Status,
                            x.CreatedAt,
                            x.DeadlineAt,
                            x.Applicant.FirstName +
                            " " +
                            x.Applicant.LastName,

                            x.CouncilAssignments.Count(a =>
                                a.AssignmentType ==
                                CouncilAssignmentType.Reviewer),

                            x.CouncilReviews.Count(),

                            x.CouncilReviews.Count(r =>
                                r.Vote == CouncilVote.Approved),

                            x.CouncilReviews.Count(r =>
                                r.Vote == CouncilVote.Rejected),

                            x.CouncilReviews.Count(r =>
                                r.Vote ==
                                CouncilVote.RequestRevision),

                            x.CouncilAssignments.Count(a =>
                                a.AssignmentType ==
                                CouncilAssignmentType.MainMember),

                            x.CouncilSignatures.Count(s =>
                                s.IsSigned)))
                    .ToListAsync(cancellationToken);

            return requests
                .Select(CreateSecretaryRequestDto)
                .ToList();
        }

        public async Task<List<SecretaryRequestDto>> GetFinalReviewRequests(
    CancellationToken cancellationToken = default)
        {
            var requests =
                await requestRepository
                    .Query()
                    .Where(x =>
                        x.Status ==
                        RequestStatus.AwaitingSecretaryFinalReview)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x =>
                        new SecretaryRequestProjection(
                            x.Id,
                            x.Title,
                            x.Description,
                            x.Urgency,
                            x.Status,
                            x.CreatedAt,
                            x.DeadlineAt,
                            x.Applicant.FirstName +
                            " " +
                            x.Applicant.LastName,

                            x.CouncilAssignments.Count(a =>
                                a.AssignmentType ==
                                CouncilAssignmentType.Reviewer),

                            x.CouncilReviews.Count(),

                            x.CouncilReviews.Count(r =>
                                r.Vote == CouncilVote.Approved),

                            x.CouncilReviews.Count(r =>
                                r.Vote == CouncilVote.Rejected),

                            x.CouncilReviews.Count(r =>
                                r.Vote ==
                                CouncilVote.RequestRevision),

                            x.CouncilAssignments.Count(a =>
                                a.AssignmentType ==
                                CouncilAssignmentType.MainMember),

                            x.CouncilSignatures.Count(s =>
                                s.IsSigned)))
                    .ToListAsync(cancellationToken);

            return requests
                .Select(CreateSecretaryRequestDto)
                .ToList();
        }
        public async Task<SecretaryRequestDetailsDto> GetReport(
    int requestId,
    CancellationToken cancellationToken = default)
        {
            return await GetRequest(
                requestId,
                cancellationToken);
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

                    .Include(x => x.CouncilAssignments)
                        .ThenInclude(x => x.CouncilMember)

                    .Include(x => x.CouncilReviews)
                        .ThenInclude(x => x.CouncilMember)

                    .Include(x => x.CouncilSignatures)
                        .ThenInclude(x => x.CouncilMember)

                    .Include(x => x.SecretaryDecisions)
                        .ThenInclude(x => x.Secretary)

                    .Include(x => x.StatusHistory)
                        .ThenInclude(x => x.ChangedByUser)

                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");

            var daysRemaining =
                CalculateDaysRemaining(request.DeadlineAt);

            var isOverdue =
                IsOverdue(request.DeadlineAt);

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
                            x.ContentType,
                            x.DocumentType))
                    .ToList();

            var reviews =
                request.CouncilReviews
                    .OrderBy(x => x.ReviewDate)
                    .Select(x =>
                        new SecretaryCouncilReviewDto(
                            x.CouncilMemberId,
                            $"{x.CouncilMember.FirstName} " +
                            $"{x.CouncilMember.LastName}",
                            x.Vote,
                            x.Comment,
                            x.ReviewDate))
                    .ToList();

            var mainMemberSignatures =
                request.CouncilAssignments
                    .Where(x =>
                        x.AssignmentType ==
                        CouncilAssignmentType.MainMember)
                    .OrderBy(x => x.CouncilMember.LastName)
                    .ThenBy(x => x.CouncilMember.FirstName)
                    .Select(x =>
                    {
                        var signature =
                            request.CouncilSignatures
                                .FirstOrDefault(s =>
                                    s.CouncilMemberId ==
                                    x.CouncilMemberId);

                        return new SecretaryMainMemberSignatureDto(
                            x.CouncilMemberId,
                            $"{x.CouncilMember.FirstName} " +
                            $"{x.CouncilMember.LastName}",
                            signature?.IsSigned ?? false,
                            signature?.SignedAt);
                    })
                    .ToList();

            var councilMembers =
                request.CouncilAssignments
                    .OrderBy(x => x.CouncilMember.LastName)
                    .ThenBy(x => x.CouncilMember.FirstName)
                    .Select(x =>
                    {
                        var review =
                            request.CouncilReviews
                                .FirstOrDefault(r =>
                                    r.CouncilMemberId ==
                                    x.CouncilMemberId);

                        var signature =
                            request.CouncilSignatures
                                .FirstOrDefault(s =>
                                    s.CouncilMemberId ==
                                    x.CouncilMemberId);

                        return new SecretaryCouncilMemberDto(
                            x.CouncilMemberId,
                            $"{x.CouncilMember.FirstName} " +
                            $"{x.CouncilMember.LastName}",
                            x.AssignmentType,
                            review is not null,
                            review?.Vote,
                            review?.ReviewDate,
                            signature?.IsSigned ?? false,
                            signature?.SignedAt);
                    })
                    .ToList();

            var secretaryDecisions =
                request.SecretaryDecisions
                    .OrderBy(x => x.CreatedAt)
                    .Select(x =>
                        new SecretaryDecisionHistoryDto(
                            x.SecretaryId,
                            $"{x.Secretary.FirstName} " +
                            $"{x.Secretary.LastName}",
                            x.Stage,
                            x.Decision,
                            x.Comment,
                            x.CreatedAt))
                    .ToList();

            var statusHistory =
                request.StatusHistory
                    .OrderBy(x => x.ChangedAt)
                    .Select(x =>
                        new SecretaryStatusHistoryDto(
                            x.Id,
                            x.FromStatus,
                            x.ToStatus,
                            x.ChangedByUserId,
                            x.ChangedByUser == null
                                ? null
                                : $"{x.ChangedByUser.FirstName} " +
                                  $"{x.ChangedByUser.LastName}",
                            x.ChangedAt,
                            x.Comment))
                    .ToList();

            return new SecretaryRequestDetailsDto(
                request.Id,
                request.Title,
                request.Description,
                request.Urgency,
                request.Status,
                request.CreatedAt,
                request.DeadlineAt,
                daysRemaining,
                isOverdue,
                applicant,
                documents,
                councilMembers,
                reviews,
                mainMemberSignatures,
                secretaryDecisions,
                statusHistory);
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
                    .Include(x => x.CouncilAssignments)
                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            var currentStatus =
                request.Status;

            var isInitialReview =
                currentStatus ==
                RequestStatus.AwaitingSecretaryInitialReview;

            var isFinalReview =
                currentStatus ==
                RequestStatus.AwaitingSecretaryFinalReview;

            if (!isInitialReview && !isFinalReview)
            {
                throw new InvalidOperationException(
                    "این درخواست در مرحله بررسی دبیر نیست.");
            }

            if (isInitialReview)
            {
                await SubmitInitialDecision(
                    request,
                    secretaryId,
                    dto,
                    cancellationToken);
            }
            else
            {
                await SubmitFinalDecision(
                    request,
                    secretaryId,
                    dto,
                    cancellationToken);
            }

            await unitOfWork.SaveChangesAsync();
        }

        private async Task SubmitInitialDecision(
            Request request,
            int secretaryId,
            SecretaryDecisionDto dto,
            CancellationToken cancellationToken)
        {
            if (dto.Decision ==
                SecretaryDecisionType.Rejected)
            {
                throw new InvalidOperationException(
                    "در بررسی اولیه دبیر امکان رد درخواست وجود ندارد.");
            }

            var newStatus =
                dto.Decision switch
                {
                    SecretaryDecisionType.Approved =>
                        RequestStatus.UnderCouncilReview,

                    SecretaryDecisionType.NeedsRevision =>
                        RequestStatus.NeedsRevision,

                    _ => throw new InvalidOperationException(
                        "تصمیم دبیر معتبر نیست.")
                };

            await secretaryDecisionRepository.AddAsync(
                new SecretaryDecision
                {
                    RequestId = request.Id,
                    SecretaryId = secretaryId,
                    Stage = SecretaryDecisionStage.InitialReview,
                    Decision = dto.Decision,
                    Comment = dto.Comment?.Trim(),
                    CreatedAt = DateTime.UtcNow
                });

            var oldStatus =
                request.Status;

            request.Status =
                newStatus;

            await workflowService.ChangeStatus(
    request,
    newStatus,
    secretaryId,
    dto.Comment);

            if (newStatus ==
                RequestStatus.UnderCouncilReview)
            {
                await AssignCouncilReviewers(
                    request,
                    cancellationToken);
            }
        }

        private async Task SubmitFinalDecision(
    Request request,
    int secretaryId,
    SecretaryDecisionDto dto,
    CancellationToken cancellationToken)
        {
            var newStatus = dto.Decision switch
            {
                SecretaryDecisionType.Approved =>
                    RequestStatus.AwaitingMainCouncilApproval,

                SecretaryDecisionType.Rejected =>
                    RequestStatus.Rejected,

                SecretaryDecisionType.NeedsRevision =>
                    RequestStatus.NeedsRevision,

                _ => throw new InvalidOperationException(
                    "تصمیم دبیر معتبر نیست.")
            };

            if (dto.Decision == SecretaryDecisionType.Approved)
            {
                await AssignMainCouncilMembers(
                    request,
                    cancellationToken);
            }

            var comment = dto.Comment?.Trim();

            await secretaryDecisionRepository.AddAsync(
                new SecretaryDecision
                {
                    RequestId = request.Id,
                    SecretaryId = secretaryId,
                    Stage = SecretaryDecisionStage.FinalReview,
                    Decision = dto.Decision,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                });

            var oldStatus = request.Status;

            request.Status = newStatus;

            await workflowService.ChangeStatus(
    request,
    newStatus,
    secretaryId,
    comment);
        }
        private async Task AssignMainCouncilMembers(
    Request request,
    CancellationToken cancellationToken)
        {
            var mainMemberIds =
                await councilMemberRepository
                    .Query()
                    .Where(x =>
                        x.IsActive &&
                        x.Type == CouncilMemberType.MainMember)
                    .Select(x => x.UserId)
                    .ToListAsync(cancellationToken);

            if (mainMemberIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "هیچ عضو اصلی فعال برای امضای نهایی شورا تعریف نشده است.");
            }

            var existingAssignmentIds =
                request.CouncilAssignments
                    .Select(x => x.CouncilMemberId)
                    .ToHashSet();

            foreach (var memberId in mainMemberIds)
            {
                if (existingAssignmentIds.Contains(memberId))
                    continue;

                await assignmentRepository.AddAsync(
                    new RequestCouncilAssignment
                    {
                        RequestId = request.Id,
                        CouncilMemberId = memberId,
                        AssignmentType =
                            CouncilAssignmentType.MainMember,
                        AssignedAt = DateTime.UtcNow
                    });

                await councilSignatureRepository.AddAsync(
                    new CouncilSignature
                    {
                        RequestId = request.Id,
                        CouncilMemberId = memberId,
                        IsSigned = false,
                        SignedAt = null
                    });
            }
        }
        private async Task AssignCouncilReviewers(
    Request request,
    CancellationToken cancellationToken)
        {
            var reviewerIds =
                await councilMemberRepository
                    .Query()
                    .Where(x =>
                        x.IsActive &&
                        x.Type == CouncilMemberType.Reviewer)
                    .Select(x => x.UserId)
                    .ToListAsync(cancellationToken);

            if (reviewerIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "هیچ عضو فعال بررسی‌کننده‌ای برای شورا تعریف نشده است.");
            }

            var existingAssignments =
                request.CouncilAssignments
                    .Select(x => x.CouncilMemberId)
                    .ToHashSet();

            foreach (var reviewerId in reviewerIds)
            {
                if (existingAssignments.Contains(reviewerId))
                {
                    continue;
                }

                request.CouncilAssignments.Add(
                    new RequestCouncilAssignment
                    {
                        RequestId = request.Id,
                        CouncilMemberId = reviewerId,
                        AssignmentType =
                            CouncilAssignmentType.Reviewer,
                        AssignedAt = DateTime.UtcNow
                    });
            }
        }

        private static SecretaryRequestDto CreateSecretaryRequestDto(
    SecretaryRequestProjection request)
        {
            var daysRemaining =
                CalculateDaysRemaining(request.DeadlineAt);

            var isOverdue =
                IsOverdue(request.DeadlineAt);

            var pendingVotes =
                Math.Max(
                    0,
                    request.TotalCouncilReviewers -
                    request.SubmittedVotes);

            var pendingMainMemberSignatures =
                Math.Max(
                    0,
                    request.TotalMainMembers -
                    request.SignedMainMembers);

            return new SecretaryRequestDto(
                request.Id,
                request.Title,
                request.Description,
                request.Urgency,
                request.Status,
                request.CreatedAt,
                request.DeadlineAt,
                daysRemaining,
                isOverdue,
                request.ApplicantFullName,
                request.TotalCouncilReviewers,
                request.SubmittedVotes,
                pendingVotes,
                request.ApprovedVotes,
                request.RejectedVotes,
                request.NeedsRevisionVotes,
                request.TotalMainMembers,
                request.SignedMainMembers,
                pendingMainMemberSignatures);
        }

        private static int CalculateDaysRemaining(
            DateTime deadlineAt)
        {
            var remaining =
                deadlineAt - DateTime.UtcNow;

            return (int)Math.Ceiling(
                remaining.TotalDays);
        }

        private static bool IsOverdue(
            DateTime deadlineAt)
        {
            return deadlineAt < DateTime.UtcNow;
        }

        private string BuildFileUrl(
            string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return string.Empty;
            }

            return
                $"{settings.S3Storage.BaseUrlWithBucket.TrimEnd('/')}/{fileUrl.TrimStart('/')}";
        }

        private sealed record SecretaryRequestProjection(
    int Id,
    string Title,
    string Description,
    UrgencyLevel Urgency,
    RequestStatus Status,
    DateTime CreatedAt,
    DateTime DeadlineAt,
    string ApplicantFullName,
    int TotalCouncilReviewers,
    int SubmittedVotes,
    int ApprovedVotes,
    int RejectedVotes,
    int NeedsRevisionVotes,
    int TotalMainMembers,
    int SignedMainMembers);
    }
}