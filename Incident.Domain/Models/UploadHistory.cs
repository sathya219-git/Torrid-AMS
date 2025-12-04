namespace Incident.Domain.Models
{
    public class UploadHistory
    {
        public int ID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public DateTime Uploaded_At { get; set; }
        public int TotalCount { get; set; }
    }
}
