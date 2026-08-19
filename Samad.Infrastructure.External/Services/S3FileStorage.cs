using Amazon.S3;
using Amazon.S3.Model;
using Samad.Application.Dtos;
using Samad.Application.IServices;

namespace Samad.Infrastructure.External.Services
{
    public sealed class S3FileStorage(
                                    IAmazonS3 s3,
                                    AppSettings options) : IFileStorage
    {
        public async Task SaveAsync(
            StorageObject file,
            CancellationToken cancellationToken = default)
        {
            if (file.Stream.CanSeek)
                file.Stream.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = options.S3Storage.BucketName,
                Key = file.Key,
                InputStream = file.Stream,
                ContentType = file.ContentType,

                AutoCloseStream = false
            };

            await s3.PutObjectAsync(request, cancellationToken);
        }

        public async Task DeleteAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = options.S3Storage.BucketName,
                Key = key
            };

            await s3.DeleteObjectAsync(request, cancellationToken);
        }
    }
}
