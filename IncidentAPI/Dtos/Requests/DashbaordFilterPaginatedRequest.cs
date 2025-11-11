namespace Incident.API.Dtos.Requests
{
    public class DashboardFilterPaginatedRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<string>? AssignmentGroup { get; set; }
        public List<string>? Category { get; set; }
        public List<string>? Priority { get; set; }
        public List<string>? AssignedToName { get; set; }
        public List<string>? State { get; set; }
        public string? Search { get; set; }  
        public string? SortBy { get; set; } 
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 
        public List<string>? IncidentNumber { get; set; }
        public List<string>? ActualResolvedTime { get; set; }
        public List<string>? BreachSLA { get; set; }
    }
}
