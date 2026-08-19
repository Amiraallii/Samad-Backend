namespace Samad.Application.Dtos
{
    public sealed record ProcessedFile(
    Stream Stream,
    long Length,
    string Extension,
    string ContentType,
    bool OwnsStream);
}
