using Microsoft.EntityFrameworkCore;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;
using Samad.Infrastructure.IRepositories;

namespace Samad.Application.Services
{
    public sealed class CouncilSignatureService(
    IRepository<Request, int> requestRepository,
    IRepository<CouncilSignature, int> signatureRepository,
    IRepository<RequestCouncilAssignment, int> assignmentRepository,
    IRequestWorkflowService workflowService,
    IUnitOfWork unitOfWork)
        : ICouncilSignatureService
    {
        public async Task<List<CouncilSignatureDto>> GetAssignedRequests(
            int councilMemberId,
            CancellationToken cancellationToken = default)
        {
            return await signatureRepository
                .Query()
                .Where(x =>
                    x.CouncilMemberId == councilMemberId &&
                    x.Request.Status ==
                    RequestStatus.AwaitingMainCouncilApproval)
                .OrderByDescending(x => x.Request.CreatedAt)
                .Select(x =>
    new CouncilSignatureDto(
        x.RequestId,
        x.CouncilMemberId,
        x.CouncilMember.FirstName +
        " " +
        x.CouncilMember.LastName,
        x.IsSigned,
        x.SignedAt))
                .ToListAsync(cancellationToken);
        }

        public async Task<CouncilSignatureDetailsDto> GetRequest(
            int requestId,
            int councilMemberId,
            CancellationToken cancellationToken = default)
        {
            var request =
                await requestRepository
                    .Query()
                    .Include(x => x.CouncilAssignments)
                        .ThenInclude(x => x.CouncilMember)
                    .Include(x => x.CouncilSignatures)
                        .ThenInclude(x => x.CouncilMember)
                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            var isAssigned =
                request.CouncilAssignments.Any(x =>
                    x.CouncilMemberId == councilMemberId &&
                    x.AssignmentType ==
                    CouncilAssignmentType.MainMember);

            if (!isAssigned)
            {
                throw new UnauthorizedAccessException(
                    "این درخواست برای امضای شما تخصیص داده نشده است.");
            }

            if (request.Status !=
                RequestStatus.AwaitingMainCouncilApproval)
            {
                throw new InvalidOperationException(
                    "این درخواست در مرحله امضای شورای اصلی نیست.");
            }

            var signatures =
                request.CouncilAssignments
                    .Where(x =>
                        x.AssignmentType ==
                        CouncilAssignmentType.MainMember)
                    .OrderBy(x =>
                        x.CouncilMember.LastName)
                    .ThenBy(x =>
                        x.CouncilMember.FirstName)
                    .Select(x =>
                    {
                        var signature =
                            request.CouncilSignatures
                                .FirstOrDefault(s =>
                                    s.CouncilMemberId ==
                                    x.CouncilMemberId);

                        return new CouncilSignatureDto(
                            x.RequestId,
                            x.CouncilMemberId,
                            x.CouncilMember.FirstName +
                            " " +
                            x.CouncilMember.LastName,
                            signature?.IsSigned ?? false,
                            signature?.SignedAt);
                    })
                    .ToList();

            return new CouncilSignatureDetailsDto(
                request.Id,
                request.Title,
                request.Description,
                signatures);
        }

        public async Task Sign(
    int requestId,
    int councilMemberId,
    CouncilSignatureRequestDto dto,
    CancellationToken cancellationToken = default)
        {
            if (!dto.Confirm)
            {
                throw new InvalidOperationException(
                    "برای ثبت امضا باید تایید کنید.");
            }

            var request =
                await requestRepository
                    .QueryTracking()
                    .Include(x => x.CouncilAssignments)
                    .Include(x => x.CouncilSignatures)
                    .SingleOrDefaultAsync(
                        x => x.Id == requestId,
                        cancellationToken);

            if (request is null)
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            if (request.Status !=
                RequestStatus.AwaitingMainCouncilApproval)
            {
                throw new InvalidOperationException(
                    "این درخواست در مرحله امضای شورای اصلی نیست.");
            }

            var assignment =
                request.CouncilAssignments
                    .FirstOrDefault(x =>
                        x.CouncilMemberId == councilMemberId &&
                        x.AssignmentType ==
                        CouncilAssignmentType.MainMember);

            if (assignment is null)
            {
                throw new UnauthorizedAccessException(
                    "این درخواست برای امضای شما تخصیص داده نشده است.");
            }

            var signature =
                request.CouncilSignatures
                    .FirstOrDefault(x =>
                        x.CouncilMemberId == councilMemberId);

            if (signature is null)
            {
                throw new InvalidOperationException(
                    "رکورد امضای شما برای این درخواست یافت نشد.");
            }

            if (signature.IsSigned)
            {
                throw new InvalidOperationException(
                    "امضای شما قبلاً ثبت شده است.");
            }

            signature.IsSigned = true;
            signature.SignedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            await CompleteRequestIfAllMainMembersSigned(
                requestId,
                cancellationToken);
        }

        private async Task CompleteRequestIfAllMainMembersSigned(
            int requestId,
            CancellationToken cancellationToken)
        {
            var requiredSignatures =
                await assignmentRepository
                    .Query()
                    .CountAsync(
                        x =>
                            x.RequestId == requestId &&
                            x.AssignmentType ==
                            CouncilAssignmentType.MainMember,
                        cancellationToken);

            var signedCount =
                await signatureRepository
                    .Query()
                    .CountAsync(
                        x =>
                            x.RequestId == requestId &&
                            x.IsSigned,
                        cancellationToken);

            if (requiredSignatures == 0 ||
                signedCount < requiredSignatures)
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
                RequestStatus.AwaitingMainCouncilApproval)
            {
                return;
            }

            await workflowService.ChangeStatus(
     request,
     RequestStatus.Approved,
     null,
     "تمام اعضای اصلی شورا درخواست را امضا کردند.");

            await unitOfWork.SaveChangesAsync();
        }
    }
}