using Samad.Application.Dtos;
using Samad.Domain.Enum;

namespace Samad.Application.Files
{
    public static class FileTypeCatalog
    {
        private static readonly Dictionary<string, (FileCategory Category, string ContentType)> Types =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [".jpg"] = (FileCategory.Image, "image/jpeg"),
                [".jpeg"] = (FileCategory.Image, "image/jpeg"),
                [".png"] = (FileCategory.Image, "image/png"),
                [".webp"] = (FileCategory.Image, "image/webp"),
                [".svg"] = (FileCategory.Image, "image/svg+xml"),
                [".heic"] = (FileCategory.Image, "image/heic"),

                [".pdf"] = (FileCategory.Pdf, "application/pdf"),

                [".mp4"] = (FileCategory.Video, "video/mp4"),
                [".mov"] = (FileCategory.Video, "video/quicktime"),
                [".flv"] = (FileCategory.Video, "video/x-flv"),

                [".mp3"] = (FileCategory.Audio, "audio/mpeg"),
                [".wav"] = (FileCategory.Audio, "audio/wav"),
                [".m4a"] = (FileCategory.Audio, "audio/mp4"),

                [".zip"] = (FileCategory.Archive, "application/zip"),
                [".rar"] = (FileCategory.Archive, "application/vnd.rar"),
                [".7z"] = (FileCategory.Archive, "application/x-7z-compressed"),

                [".apk"] = (FileCategory.Binary, "application/vnd.android.package-archive"),
                [".exe"] = (FileCategory.Binary, "application/octet-stream"),
                [".dll"] = (FileCategory.Binary, "application/octet-stream")
            };

        public static DetectedFile Detect(string originalFileName)
        {
            var fileName = Path.GetFileName(originalFileName);
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension))
                throw new InvalidOperationException("فایل extension ندارد.");

            if (!Types.TryGetValue(extension, out var info))
                throw new InvalidOperationException(
                    $"نوع فایل '{extension}' پشتیبانی نمی‌شود.");

            return new DetectedFile(
                info.Category,
                extension,
                info.ContentType);
        }
    }
}
