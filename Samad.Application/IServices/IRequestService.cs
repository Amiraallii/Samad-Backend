using Samad.Application.Dtos;

namespace Samad.Application.IServices
{
    public interface IRequestService
    {
        Task AddRequest(NewRequest dto);
        Task<List<RequestListDto>> GetMyRequests(
    int applicantId,
    CancellationToken ct = default);

        Task<RequestDetailsDto> GetMyRequest(
    int requestId,
    int applicantId,
    CancellationToken ct = default);
        Task UpdateRequest(
    int requestId,
    int applicantId,
    UpdateRequest dto,
    CancellationToken ct = default);

        Task DeleteRequest(
    int requestId,
    int applicantId,
    CancellationToken ct = default);
    }

}
