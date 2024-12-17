using Azure;
using Azure.Data.Tables;

namespace Models.Files
{
    public class ParentRecordEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public ParentRecordEntity() { }

        public ParentRecordEntity(ParentRecord parentRecord)
        {
            PartitionKey = parentRecord.PartitionKey;
            RowKey = parentRecord.RowKey;
            Description = parentRecord.Description;
        }

        public ParentRecord ToParentRecord()
        {
            return new ParentRecord
            {
                PartitionKey = PartitionKey,
                RowKey = RowKey,
                Description = Description
            };
        }
    }
}