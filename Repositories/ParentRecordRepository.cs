using Azure.Data.Tables;
using Models.Files;
using Repositories.Interfaces;
using System.Threading.Tasks;

namespace Repositories
{
    public class ParentRecordRepository : TableRepository<ParentRecord>, IParentRecordRepository
    {
        public ParentRecordRepository(string connectionString, string tableName)
            : base(connectionString, tableName)
        {
        }

        public async Task CreateAsync(ParentRecord parentRecord)
        {
            await AddEntityAsync(parentRecord);
        }
    }
}