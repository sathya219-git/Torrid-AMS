namespace Incident.API.Dtos.Responses
{
    public class IncidentDetailsPaginatedResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public List<IncidentDetailsResponse> Incidents { get; set; } = new();
    }

    public class IncidentDetailsResponse
    {
        public string IncidentNo { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ActualResolvedTime { get; set; } = string.Empty;
        public DateTime? ResolvedDateTime { get; set; }
        public string BreachSLA { get; set; } = string.Empty;
    }
}
