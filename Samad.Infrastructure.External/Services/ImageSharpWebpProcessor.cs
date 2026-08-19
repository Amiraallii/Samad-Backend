using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Enum;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Samad.Infrastructure.External.Services
{
    public sealed class ImageSharpWebpProcessor : IFileProcessor
    {
        private static readonly HashSet<string> SupportedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
            };

        public bool CanProcess(DetectedFile file)
        {
            return file.Category == FileCategory.Image
                   && SupportedExtensions.Contains(file.Extension);
        }

        public async Task<ProcessedFile> ProcessAsync(
            IncomingFile input,
            DetectedFile detected,
            long maxOutputBytes,
            CancellationToken cancellationToken = default)
        {
            if (input.Stream.CanSeek)
                input.Stream.Position = 0;

            using var image =
                await Image.LoadAsync(input.Stream, cancellationToken);

            image.Mutate(x => x.AutoOrient());

            const int maxWidth = 1920;

            if (image.Width > maxWidth)
            {
                var scale = (double)maxWidth / image.Width;
                var newHeight = (int)(image.Height * scale);

                image.Mutate(x =>
                    x.Resize(maxWidth, newHeight));
            }

            MemoryStream? output = null;

            for (var quality = 82; quality >= 50; quality -= 8)
            {
                output?.Dispose();
                output = new MemoryStream();

                var encoder = new WebpEncoder
                {
                    Quality = quality,
                    Method = WebpEncodingMethod.BestQuality
                };

                await image.SaveAsWebpAsync(
                    output,
                    encoder,
                    cancellationToken);

                if (output.Length <= maxOutputBytes)
                    break;
            }

            if (output is null || output.Length > maxOutputBytes)
            {
                output?.Dispose();

                throw new InvalidOperationException(
                    "تصویر حتی بعد از فشرده‌سازی از حجم مجاز بزرگ‌تر است.");
            }

            output.Position = 0;

            return new ProcessedFile(
                output,
                output.Length,
                ".webp",
                "image/webp",
                OwnsStream: true);
        }
    }
}
