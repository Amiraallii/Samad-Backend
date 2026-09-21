using Samad.Application.Dtos;

namespace Samad.Application.IServices
{
    public interface ISecretaryRequestService
    {
        Task<List<SecretaryRequestDto>> GetInitialReviewRequests(
            CancellationToken cancellationToken = default);

        Task<List<SecretaryRequestDto>> GetFinalReviewRequests(
            CancellationToken cancellationToken = default);

        Task<SecretaryRequestDetailsDto> GetRequest(
            int requestId,
            CancellationToken cancellationToken = default);

        Task SubmitDecision(
            int requestId,
            int secretaryId,
            SecretaryDecisionDto dto,
            CancellationToken cancellationToken = default);

        Task<SecretaryRequestDetailsDto> GetReport(
    int requestId,
    CancellationToken cancellationToken = default);
    }
}