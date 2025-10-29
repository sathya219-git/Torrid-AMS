namespace Incident.API.Dtos.Responses
{
    public class IncidentDetailsByPriorityItemResponse
    {
        public string IncidentNo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ResolutionNotes { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public DateTime? ResolvedDateTime { get; set; }
    }
     public class IncidentDetailsByPriorityPagedResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }

        public IEnumerable<IncidentDetailsByPriorityItemResponse> Incidents { get; set; }
            = new List<IncidentDetailsByPriorityItemResponse>();
    }
}
