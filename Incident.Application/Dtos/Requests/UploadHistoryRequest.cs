namespace Incident.Application.Dtos.Requests
{
    public class UploadHistoryQueryRequest
    {
        public string? SearchText { get; set; }
        public string? SortBy { get; set; } = string.Empty;
        public string? SortDir { get; set; } = string.Empty;         
        public int PageNumber { get; set; }            
        public int PageSize { get; set; } 
    }
}
