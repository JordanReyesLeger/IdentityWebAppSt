

using Models.Files;

namespace Services.Interfaces
{
    public interface IFileService
    {
        Task CreateFileRecordAsync(FileRecord fileRecord);
        Task CreateParentRecordAsync(ParentRecord parentRecord);
        Task<(string FileUrl, string FileName, string Description)> UploadFileAsync(Stream fileStream, string fileName, string contentType, string description, int year, long consecutivo);
    }
}