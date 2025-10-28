namespace Incident.API.Dtos.Responses
{
    public class IncidentCountByPriorityGroupedResponse
    {
        public Dictionary<string, List<IncidentStateCount>> Priority { get; set; } = new();
        public string TotalAverageResolvedTime { get; set; } = string.Empty;
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
