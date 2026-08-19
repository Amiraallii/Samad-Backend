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
        bool HasReviewed);
    public record CouncilRequestDetailsDto(
        int Id,
        string Title,
        string Description,
        UrgencyLevel Urgency,
        RequestStatus Status,
        DateTime CreatedAt,
        string ApplicantFullName,
        List<CouncilRequestDocumentDto> Documents,
        bool HasReviewed);

    public record CouncilRequestDocumentDto(
        int Id,
        string FileUrl,
        string ContentType);

    public record SubmitCouncilReviewDto(
        CouncilVote Vote,
        string Comment);
}
