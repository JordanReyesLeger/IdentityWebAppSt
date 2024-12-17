
namespace Models.Files
{
    public class FileRecord
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public long FileSize { get; set; }
    }
}
