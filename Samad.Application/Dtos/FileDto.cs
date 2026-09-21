using Samad.Domain.Enum;

namespace Samad.Application.Dtos
{
    public sealed record IncomingFile(
        Stream Stream,
        string OriginalFileName,
        string? ClientContentType,
        long Length);

    public sealed record StoredFileDto(
        string Key,
        string FileName,
        string Extension,
        string ContentType,
        long Size,
        FileCategory Category);

    public sealed record DetectedFile(
        FileCategory Category,
        string Extension,
        string ContentType);
}