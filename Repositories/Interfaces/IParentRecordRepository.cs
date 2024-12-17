using Models.Files;

namespace Repositories
{
    public interface IParentRecordRepository
    {
        Task CreateAsync(ParentRecord parentRecord);
    }
}