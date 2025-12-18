namespace Incident.Application.Dtos.Responses
{
    public class IncidentCountByPriorityGroupedResponse
    {
        public Dictionary<string, PriorityData> Priority { get; set; } = new();
    }

    public class PriorityData
    {
        public Dictionary<string, long> StateDetails { get; set; } = new();
        public int TotalCountForPriority { get; set; }
        public string AvgResolvedTime { get; set; } = string.Empty;
        public string TotalResolvedTime { get; set; } = string.Empty;
        public long BreachedCount { get; set; }
        public long OpenMoreThan15Days { get; set; }
        public long OpenLessThan15Days { get; set; }
    }
}