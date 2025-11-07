namespace Incident.API.Dtos.Responses
{
    public class BreachListPagedResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public List<BreachListItemResponse> Items { get; set; } = new();
    }

    public class BreachListItemResponse
    {
        public string IncidentNumber { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ActualResolvedTime { get; set; } = string.Empty;
        public string BreachSLA { get; set; } = string.Empty;
    }
}