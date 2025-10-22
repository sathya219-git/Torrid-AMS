namespace Incident.API.Dtos.Requests
{
    public class DashboardFilterRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? AssignmentGroup { get; set; }
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public string? AssignedToName { get; set; }
    }
}
