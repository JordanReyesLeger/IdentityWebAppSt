using Models.Files;

namespace Repositories.Interfaces
{
    public interface IFileRecordRepository
    {
        Task CreateAsync(FileRecord fileRecord);
    }
}