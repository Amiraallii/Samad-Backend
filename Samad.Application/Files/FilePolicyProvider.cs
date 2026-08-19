using Samad.Domain.Enum;

namespace Samad.Application.Files
{
    public sealed record FilePolicy(
    string Folder,
    long MaxUploadBytes,
    long MaxStoredBytes);

    public sealed class FilePolicyProvider(FileUploadOptions options)
    {
        private const long Mb = 1024L * 1024L;

        public FilePolicy Get(FileCategory category)
        {
            return category switch
            {
                FileCategory.Image => new(
                    "images",
                    options.ImageMaxUploadMb * Mb,
                    options.ImageMaxStoredMb * Mb),

                FileCategory.Pdf => new(
                    "pdf",
                    options.PdfMaxUploadMb * Mb,
                    options.PdfMaxUploadMb * Mb),

                FileCategory.Video => new(
                    "videos",
                    options.VideoMaxUploadMb * Mb,
                    options.VideoMaxUploadMb * Mb),

                FileCategory.Audio => new(
                    "audios",
                    options.AudioMaxUploadMb * Mb,
                    options.AudioMaxUploadMb * Mb),

                FileCategory.Archive => new(
                    "archives",
                    options.ArchiveMaxUploadMb * Mb,
                    options.ArchiveMaxUploadMb * Mb),

                FileCategory.Binary => new(
                    "binaries",
                    options.BinaryMaxUploadMb * Mb,
                    options.BinaryMaxUploadMb * Mb),

                _ => throw new InvalidOperationException(
                    "این نوع فایل اجازه آپلود ندارد.")
            };
        }
    }
}