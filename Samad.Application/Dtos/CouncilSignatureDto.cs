namespace Samad.Application.Dtos
{
    public sealed record CouncilSignatureRequestDto(
        bool Confirm);

    public sealed record CouncilSignatureDto(
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