namespace Samad.Application.Dtos
{
    public class AppSettings
    {
        public JWTOptions Jwt { get; set; } = new();

        public S3StorageOptions S3Storage { get; set; } = new();

        public DeadlineOptions Deadline { get; set; } = new();
    }

    public class JWTOptions
    {
        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;
    }

    public class S3StorageOptions
    {
        public string Endpoint { get; set; } = string.Empty;

        public string BucketName { get; set; } = string.Empty;

        public string AccessKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public bool UseSSL { get; set; } = true;

        public bool PublicRead { get; set; } = true;

        public string ServiceUrl =>
            $"{(UseSSL ? "https" : "http")}://{Endpoint}";

        public string BaseUrlWithBucket =>
            $"{ServiceUrl}/{BucketName}/";
    }

    public class DeadlineOptions
    {
        public int NormalWeeks { get; set; } = 4;

        public int UrgentWeeks { get; set; } = 2;

        public int VeryUrgentWeeks { get; set; } = 1;
    }
}