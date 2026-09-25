namespace Samad.Application.Dtos
{
    public sealed record CouncilSignatureRequestDto(
        bool Confirm);

    public record CouncilSignatureDto(
    int RequestId,
    int CouncilMemberId,
    string CouncilMemberFullName,
    bool IsSigned,
    DateTime? SignedAt);

    public sealed record CouncilSignatureDetailsDto(
        int RequestId,
        string Title,
        string Description,
        List<CouncilSignatureDto> Signatures);
}