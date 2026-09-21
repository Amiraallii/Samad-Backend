using Samad.Application.Dtos;

namespace Samad.Application.IServices
{
    public interface ICouncilSignatureService
    {
        Task<List<CouncilSignatureDto>> GetAssignedRequests(
            int councilMemberId,
            CancellationToken cancellationToken = default);

        Task<CouncilSignatureDetailsDto> GetRequest(
            int requestId,
            int councilMemberId,
            CancellationToken cancellationToken = default);

        Task Sign(
            int requestId,
            int councilMemberId,
            CouncilSignatureRequestDto dto,
            CancellationToken cancellationToken = default);
    }
}