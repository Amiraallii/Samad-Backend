using Samad.Application.Dtos;
using Samad.Application.Files;
using Samad.Application.IServices;

namespace Samad.Application.Services
{
    public sealed class FileUploadService(
    IFileStorage storage,
    IEnumerable<IFileProcessor> processors,
    FilePolicyProvider policyProvider)
    : IFileUploadService
    {
        public async Task<StoredFileDto> SaveAsync(
            IncomingFile file,
            CancellationToken cancellationToken = default)
        {
            if (file.Stream is null ||
                !file.Stream.CanRead ||
                file.Length <= 0)
            {
                throw new ArgumentException(
                    "فایل ارسال نشده یا خالی است.");
            }

            var detected =
                FileTypeCatalog.Detect(file.OriginalFileName);

            var policy =
                policyProvider.Get(detected.Category);

            if (file.Length > policy.MaxUploadBytes)
            {
                throw new InvalidOperationException(
                    $"حجم فایل از حد مجاز بیشتر است. " +
                    $"حداکثر: {policy.MaxUploadBytes / 1024 / 1024} MB");
            }

            var processor = processors
                .FirstOrDefault(x => x.CanProcess(detected));

            ProcessedFile processed;

            if (processor is null)
            {
                processed = new ProcessedFile(
                    file.Stream,
                    file.Length,
                    detected.Extension,
                    detected.ContentType,
                    OwnsStream: false);
            }
            else
            {
                processed = await processor.ProcessAsync(
                    file,
                    detected,
                    policy.MaxStoredBytes,
                    cancellationToken);
            }

            try
            {
                if (processed.Length > policy.MaxStoredBytes)
                {
                    throw new InvalidOperationException(
                        "حجم نهایی فایل از حد مجاز بیشتر است.");
                }

                var id = Guid.NewGuid().ToString("N");
                var fileName = $"{id}{processed.Extension}";

                var key =
                    $"uploads/samad/{policy.Folder}/{DateTime.UtcNow:yyyy/MM}/{fileName}";

                if (processed.Stream.CanSeek)
                    processed.Stream.Position = 0;

                await storage.SaveAsync(
                    new StorageObject(
                        processed.Stream,
                        key,
                        processed.ContentType),
                    cancellationToken);


                return new StoredFileDto(
                    Key: key,
                    FileName: fileName,
                    Extension: processed.Extension,
                    ContentType: processed.ContentType,
                    Size: processed.Length,
                    Category: detected.Category);
            }
            finally
            {
                if (processed.OwnsStream)
                {
                    await processed.Stream.DisposeAsync();
                }
            }
        }
    }
}
