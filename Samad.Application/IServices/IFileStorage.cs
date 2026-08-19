namespace Samad.Application.IServices
{
    public sealed record StorageObject(
    Stream Stream,
    string Key,
    string ContentType);

    public interface IFileStorage
    {
        Task SaveAsync(
            StorageObject file,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string key,
            CancellationToken cancellationToken = default);
    }
}
