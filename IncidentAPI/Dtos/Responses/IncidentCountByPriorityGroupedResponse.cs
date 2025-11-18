namespace Incident.API.Dtos.Responses
{
    public class IncidentCountByPriorityGroupedResponse
    {
        public Dictionary<string, PriorityData> Priority { get; set; } = new();
    }

    public class PriorityData
    {
        public List<IncidentStateCount> Details { get; set; } = new();
        public string AvgResolvedTime { get; set; } = string.Empty;
        public string TotalResolvedTime { get; set; } = string.Empty;
        public int BreachedCount {get; set;}
    }

    public class IncidentStateCount
    {
        public int TotalCount { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Closed { get; set; }
        public int OnHold { get; set; }
        public int Reopen { get; set; }
        public int Resolved { get; set; }
    }
}
