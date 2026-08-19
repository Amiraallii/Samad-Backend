using Samad.Application.Dtos;

namespace Samad.Application.Files
{
    public interface IFileUploadService
    {
        Task<StoredFileDto> SaveAsync(
            IncomingFile file,
            CancellationToken cancellationToken = default);
    }
}
