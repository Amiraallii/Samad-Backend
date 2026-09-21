using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;
using Samad.Infrastructure.IRepositories;

namespace Samad.Application.Services
{
    public sealed class RequestWorkflowService(
        IRepository<RequestStatusHistory, int> statusHistoryRepository)
        : IRequestWorkflowService
    {
        public async Task ChangeStatus(
            Request request,
            RequestStatus newStatus,
            int? changedByUserId,
            string? comment = null)
        {
            if (request.Status == newStatus)
            {
                throw new InvalidOperationException(
                    "وضعیت جدید با وضعیت فعلی یکسان است.");
            }

            if (!IsValidTransition(
                    request.Status,
                    newStatus))
            {
                throw new InvalidOperationException(
                    $"تغییر وضعیت از «{request.Status}» به «{newStatus}» مجاز نیست.");
            }

            var oldStatus =
                request.Status;

            request.Status =
                newStatus;

            await statusHistoryRepository.AddAsync(
                new RequestStatusHistory
                {
                    RequestId = request.Id,
                    FromStatus = oldStatus,
                    ToStatus = newStatus,
                    ChangedByUserId = changedByUserId,
                    ChangedAt = DateTime.UtcNow,
                    Comment = comment?.Trim()
                });
        }

        private static bool IsValidTransition(
            RequestStatus currentStatus,
            RequestStatus newStatus)
        {
            return currentStatus switch
            {
                RequestStatus.AwaitingSecretaryInitialReview =>
                    newStatus is
                        RequestStatus.NeedsRevision or
                        RequestStatus.UnderCouncilReview,

                RequestStatus.NeedsRevision =>
                    newStatus ==
                    RequestStatus.AwaitingSecretaryInitialReview,

                RequestStatus.UnderCouncilReview =>
                    newStatus ==
                    RequestStatus.AwaitingSecretaryFinalReview,

                RequestStatus.AwaitingSecretaryFinalReview =>
                    newStatus is
                        RequestStatus.NeedsRevision or
                        RequestStatus.Rejected or
                        RequestStatus.AwaitingMainCouncilApproval,

                RequestStatus.AwaitingMainCouncilApproval =>
                    newStatus ==
                    RequestStatus.Approved,

                RequestStatus.Approved =>
                    false,

                RequestStatus.Rejected =>
                    false,

                _ => false
            };
        }
    }
}