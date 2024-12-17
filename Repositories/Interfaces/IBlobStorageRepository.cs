namespace Repositories.Interfaces
{
    public interface IBlobStorageRepository
    {
        Task DeleteFileAsync(string fileName);
        Task<Stream> DownloadFileAsync(string fileName);
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, int year, long id);
    }
}