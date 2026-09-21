using Samad.Domain.Enum;

namespace Samad.Application.Dtos
{
    public record CouncilRequestDto(
        int Id,
        string Title,
        string Description,
        UrgencyLevel Urgency,
        RequestStatus Status,
        DateTime CreatedAt,
        DateTime DeadlineAt,
        bool HasReviewed);

    public record CouncilRequestDetailsDto(
        int Id,
        string Title,
        string Description,
        UrgencyLevel Urgency,
        RequestStatus Status,
        DateTime CreatedAt,
        DateTime DeadlineAt,
        string ApplicantFullName,
        List<CouncilRequestDocumentDto> Documents,
        bool HasReviewed);

    public sealed record CouncilRequestDocumentDto(
        int Id,
        string FileUrl,
        string ContentType,
        DocumentType DocumentType);

    public record SubmitCouncilReviewDto(
        CouncilVote Vote,
        string? Comment);
}