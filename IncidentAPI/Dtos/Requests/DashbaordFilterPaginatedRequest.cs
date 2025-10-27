namespace Incident.API.Dtos.Requests
{
    public class DashboardFilterPaginatedRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Category { get; set; }
        public string? AssignmentGroup { get; set; }
        public string? Priority { get; set; }
        public string? AssignedToName { get; set; }
        public string? State { get; set; }
        public string? Search { get; set; }  
        public string? SortBy { get; set; } 
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 
    }
}
