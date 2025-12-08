namespace Incident.Domain.Entities
{
    public class FileIngestionResult
    {
        public long UploadID { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string CsvPath { get; set; } = string.Empty;
        public string Message { get; set; } = "Success";
    }
}