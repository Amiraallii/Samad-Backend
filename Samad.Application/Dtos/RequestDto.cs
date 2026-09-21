using Samad.Domain.Enum;

namespace Samad.Application.Dtos
{
    public class RequestDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public UrgencyLevel Urgency { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int ApplicantId { get; set; }

        public int? SecretaryId { get; set; }

        public string? FinalSecretaryComment { get; set; }

        public DateTime? FinalDecisionDate { get; set; }
    }

    public sealed record NewRequest(
        string Title,
        string Description,
        UrgencyLevel Urgency,
        int ApplicantId,
        List<IncomingDocument> Documents);

    public sealed record RequestListDto(
    int Id,
    string Title,
    string Description,
    UrgencyLevel Urgency,
    RequestStatus Status,
    DateTime CreatedAt,
    DateTime DeadlineAt,
    int DaysRemaining,
    bool IsOverdue);

    public sealed record RequestDocumentDto(
        int Id,
        string FileUrl,
        string ContentType,
        DocumentType DocumentType);

    public sealed record RequestDetailsDto(
    int Id,
    string Title,
    string Description,
    UrgencyLevel Urgency,
    RequestStatus Status,
    DateTime CreatedAt,
    DateTime DeadlineAt,
    int DaysRemaining,
    bool IsOverdue,
    List<RequestDocumentDto> Documents);

    public sealed record UpdateRequest(
        string Title,
        string Description,
        UrgencyLevel Urgency,
        List<int> RemoveDocumentIds,
        List<IncomingDocument> Documents);

    public sealed record IncomingDocument(
        IncomingFile File,
        DocumentType DocumentType);
}