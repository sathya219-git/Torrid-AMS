namespace Incident.Application.Dtos.Requests
{
    public class DashboardFilterPaginatedRequest : DashboardFilterRequest
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10; 
        public List<string>? IncidentNumber { get; set; }
        public List<string>? ActualResolvedTime { get; set; }
        public List<string>? BreachSLA { get; set; }
    }
}