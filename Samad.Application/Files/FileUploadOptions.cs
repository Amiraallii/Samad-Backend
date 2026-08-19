namespace Samad.Application.Files
{
    public sealed class FileUploadOptions
    {
        public int ImageMaxUploadMb { get; init; } = 3;
        public int ImageMaxStoredMb { get; init; } = 1;

        public int PdfMaxUploadMb { get; init; } = 10;
        public int VideoMaxUploadMb { get; init; } = 5;
        public int AudioMaxUploadMb { get; init; } = 5;
        public int ArchiveMaxUploadMb { get; init; } = 15;
        public int BinaryMaxUploadMb { get; init; } = 15;
    }
}
