namespace Incident.API.DTOs.Responses
{
    public class PagedUploadHistoryResponse
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<UploadHistoryItemResponse> Items { get; set; } = new();
    }
     public class UploadHistoryItemResponse
    {
        public int ID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
    }
}
