using Azure.Data.Tables;
using Models.Files;
using System.Threading.Tasks;

namespace Repositories
{
    public abstract class TableRepository<T> where T : class, new()
    {
        protected readonly TableClient _tableClient;

        protected TableRepository(string connectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        protected async Task AddEntityAsync(T entity)
        {
            if (entity is FileRecord fileRecord)
            {
                var fileRecordEntity = new FileRecordEntity(fileRecord);
                await _tableClient.UpsertEntityAsync(fileRecordEntity, TableUpdateMode.Replace);
            }
            else if (entity is ParentRecord parentRecord)
            {
                var parentRecordEntity = new ParentRecordEntity(parentRecord);
                await _tableClient.UpsertEntityAsync(parentRecordEntity, TableUpdateMode.Replace);
            }
            else
            {
                // Handle other types if necessary
            }
        }

        protected async Task<T> GetEntityAsync(string partitionKey, string rowKey)
        {
            if (typeof(T) == typeof(FileRecord))
            {
                var response = await _tableClient.GetEntityAsync<FileRecordEntity>(partitionKey, rowKey);
                return response.Value.ToFileRecord() as T;
            }
            else if (typeof(T) == typeof(ParentRecord))
            {
                var response = await _tableClient.GetEntityAsync<ParentRecordEntity>(partitionKey, rowKey);
                return response.Value.ToParentRecord() as T;
            }
            else
            {
                // Handle other types if necessary
                return null;
            }
        }

        /// <summary>
        /// Recupera una lista de entidades de la tabla usando solo la clave de partición.
        /// </summary>
        /// <param name="partitionKey">La clave de partición de las entidades.</param>
        /// <returns>Una lista de entidades recuperadas.</returns>
        protected async Task<List<T>> GetEntityByPartitionKeyAsync(string partitionKey)
        {
            var query = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'");

            var entities = new List<T>();
            await foreach (var entity in query)
            {
                if (typeof(T) == typeof(FileRecord))
                {
                    var fileRecordEntity = new FileRecordEntity
                    {
                        PartitionKey = entity.PartitionKey,
                        RowKey = entity.RowKey,
                        // Map other properties as needed
                    };
                    entities.Add(fileRecordEntity.ToFileRecord() as T);
                }
                else if (typeof(T) == typeof(ParentRecord))
                {
                    var parentRecordEntity = new ParentRecordEntity
                    {
                        PartitionKey = entity.PartitionKey,
                        RowKey = entity.RowKey,
                        // Map other properties as needed
                    };
                    entities.Add(parentRecordEntity.ToParentRecord() as T);
                }
                else
                {
                    // Manejar otros tipos si es necesario
                }
            }

            return entities;
        }

        protected async Task DeleteEntityAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}
