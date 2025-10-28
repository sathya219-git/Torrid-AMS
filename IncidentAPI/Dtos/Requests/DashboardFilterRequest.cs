namespace Incident.API.Dtos.Requests
{
    public class DashboardFilterRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<string>? AssignmentGroup { get; set; }
        public List<string>? Category { get; set; }
        public List<string>? Priority { get; set; }
        public List<string>? AssignedToName { get; set; }
        public List<string>? State { get; set; }
        public string? Metrics {get; set;}
    }
}
