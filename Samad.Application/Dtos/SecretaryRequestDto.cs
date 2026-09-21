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
        DateTime DeadlineAt,
        int DaysRemaining,
        bool IsOverdue,
        string ApplicantFullName,
        int TotalCouncilReviewers,
        int SubmittedVotes,
        int PendingVotes,
        int ApprovedVotes,
        int RejectedVotes,
        int NeedsRevisionVotes,
        int TotalMainMembers,
        int SignedMainMembers,
        int PendingMainMemberSignatures);

    public record SecretaryRequestDetailsDto(
        int Id,
        string Title,
        string Description,
        UrgencyLevel Urgency,
        RequestStatus Status,
        DateTime CreatedAt,
        DateTime DeadlineAt,
        int DaysRemaining,
        bool IsOverdue,
        SecretaryApplicantDto Applicant,
        List<SecretaryDocumentDto> Documents,
        List<SecretaryCouncilMemberDto> CouncilMembers,
        List<SecretaryCouncilReviewDto> CouncilReviews,
        List<SecretaryMainMemberSignatureDto> MainMemberSignatures,
        List<SecretaryDecisionHistoryDto> SecretaryDecisions,
        List<SecretaryStatusHistoryDto> StatusHistory);

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
        string ContentType,
        DocumentType DocumentType);

    public record SecretaryCouncilMemberDto(
        int CouncilMemberId,
        string CouncilMemberFullName,
        CouncilAssignmentType AssignmentType,
        bool HasVoted,
        CouncilVote? Vote,
        DateTime? ReviewDate,
        bool HasSigned,
        DateTime? SignedAt);

    public record SecretaryCouncilReviewDto(
        int CouncilMemberId,
        string CouncilMemberFullName,
        CouncilVote Vote,
        string? Comment,
        DateTime ReviewDate);

    public record SecretaryMainMemberSignatureDto(
        int CouncilMemberId,
        string CouncilMemberFullName,
        bool IsSigned,
        DateTime? SignedAt);

    public record SecretaryDecisionDto(
        SecretaryDecisionType Decision,
        string? Comment);

    public record SecretaryDecisionHistoryDto(
        int SecretaryId,
        string SecretaryFullName,
        SecretaryDecisionStage Stage,
        SecretaryDecisionType Decision,
        string? Comment,
        DateTime CreatedAt);

    public record SecretaryStatusHistoryDto(
        int Id,
        RequestStatus? FromStatus,
        RequestStatus ToStatus,
        int? ChangedByUserId,
        string? ChangedByUserFullName,
        DateTime ChangedAt,
        string? Comment);
}