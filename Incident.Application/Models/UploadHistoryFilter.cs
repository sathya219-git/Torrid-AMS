namespace Incident.Application.Filters
{
    public class UploadHistoryFilter
    {
        public string? SearchText { get; set; }
        public string? SortBy { get; set; } = "UploadedDate";
        public string? SortDir { get; set; } = "DESC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 4;
    }
}
