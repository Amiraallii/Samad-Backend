namespace Samad.Application.Dtos
{
    public class AppSettings
    {
        public JWTOptions Jwt { get; set; } = new();
        public S3StorageOptions S3Storage { get; set; } = new();
    }
    public class JWTOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
    }
    public class S3StorageOptions
    {
        public string Endpoint { get; set; }
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool UseSSL { get; set; } = true;
        public bool PublicRead { get; set; } = true;

        public string ServiceUrl => $"{(UseSSL ? "https" : "http")}://{Endpoint}";
        public string BaseUrlWithBucket => $"{ServiceUrl}/{BucketName}/";
    }
}
