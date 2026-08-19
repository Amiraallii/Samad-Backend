using Samad.Domain.Enum;

namespace Samad.Application.Dtos
{
    public record SecretaryRequestDto(
        int Id,
        string Title,
        string Description,
        UrgencyLevel Urgency,
        RequestStatus Status,
        DateTime CreatedAt,
        string ApplicantFullName,
        int TotalCouncilMembers,
        int ApprovedVotes,
        int RejectedVotes,
        int NeedsRevisionVotes);
    public record SecretaryRequestDetailsDto(
        int Id,
        string Title,
        string Description,
        UrgencyLevel Urgency,
        RequestStatus Status,
        DateTime CreatedAt,
        SecretaryApplicantDto Applicant,
        List<SecretaryDocumentDto> Documents,
        List<SecretaryCouncilReviewDto> CouncilReviews);

    public record SecretaryApplicantDto(
        int Id,
        string FirstName,
        string LastName,
        string NationalCode,
        string Email,
        string PhoneNumber);

    public record SecretaryDocumentDto(
        int Id,
        string FileUrl,
        string ContentType);

    public record SecretaryCouncilReviewDto(
        int CouncilMemberId,
        string CouncilMemberFullName,
        CouncilVote Vote,
        string Comment,
        DateTime ReviewDate);
    public record SecretaryDecisionDto(
      SecretaryDecision Decision,
      string Comment);
}
