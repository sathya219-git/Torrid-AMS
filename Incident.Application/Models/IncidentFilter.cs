namespace Incident.Application.Models
{
    public class IncidentFilter
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
    }
}
