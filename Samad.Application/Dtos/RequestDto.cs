using Samad.Domain.Enum;

namespace Samad.Application.Dtos
{
    public class RequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UrgencyLevel Urgency { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ApplicantId { get; set; }

        public int? SecretaryId { get; set; }
        public string? FinalSecretaryComment { get; set; }
        public DateTime? FinalDecisionDate { get; set; }
    }

    public record NewRequest(string Title,
        string Description,
        UrgencyLevel Urgency,
        int ApplicantId,
        List<IncomingFile> files);
    public record RequestListDto(
    int Id,
    string Title,
    string Description,
    UrgencyLevel Urgency,
    RequestStatus Status,
    DateTime CreatedAt);
    public record RequestDocumentDto(
    int Id,
    string FileUrl,
    string ContentType);

    public record RequestDetailsDto(
        int Id,
        string Title,
        string Description,
        UrgencyLevel Urgency,
        RequestStatus Status,
        DateTime CreatedAt,
        List<RequestDocumentDto> Documents);


    public record UpdateRequest(
        string Title,
        string Description,
        UrgencyLevel Urgency,
        List<int> RemoveDocumentIds,
        List<IncomingFile> Files);
}
