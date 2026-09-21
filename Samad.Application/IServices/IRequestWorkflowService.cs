using Samad.Domain.Entity;
using Samad.Domain.Enum;

namespace Samad.Application.IServices
{
    public interface IRequestWorkflowService
    {
        Task ChangeStatus(
            Request request,
            RequestStatus newStatus,
            int? changedByUserId,
            string? comment = null);
    }
}