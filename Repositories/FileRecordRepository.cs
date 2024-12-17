using Azure.Data.Tables;
using Models.Files;
using Repositories.Interfaces;
using System.Threading.Tasks;

namespace Repositories
{
    public class FileRecordRepository : TableRepository<FileRecord>, IFileRecordRepository
    {
        public FileRecordRepository(string connectionString, string tableName)
            : base(connectionString, tableName)
        {
        }

        public async Task CreateAsync(FileRecord fileRecord)
        {
            await AddEntityAsync(fileRecord);
        }
    }
}
