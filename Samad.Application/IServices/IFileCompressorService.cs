namespace Samad.Application.IServices
{
    public interface IFileCompressorService
    {
        Task<Stream> CompressImageAsync(Stream inputStream, int quality = 80, int maxWidth = 1920);
    }
}
