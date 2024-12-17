using Azure;
using Azure.Data.Tables;
using Models.Files;

public class FileRecordEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string Url { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public FileRecordEntity() { }

    public FileRecordEntity(FileRecord fileRecord)
    {
        PartitionKey = fileRecord.PartitionKey;
        RowKey = fileRecord.RowKey;
        FileName = fileRecord.FileName;
        FileSize = fileRecord.FileSize;
        Url = fileRecord.Url;
    }

    public FileRecord ToFileRecord()
    {
        return new FileRecord
        {
            PartitionKey = PartitionKey,
            RowKey = RowKey,
            FileName = FileName,
            Url = Url,
            FileSize = FileSize
        };
    }
}