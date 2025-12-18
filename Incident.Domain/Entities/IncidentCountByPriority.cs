namespace Incident.Domain.Entities
{
    public class IncidentCountByPriority
    {
        public string Priority { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public long IncidentCount { get; set; }
        public long TotalCount { get; set; }

        public string AvgResolvedTime { get; set; } = string.Empty;
        public string TotalResolvedTime { get; set; } = string.Empty;

        public long BreachedCount { get; set; }
        public long Open_more_15_days { get; set; }
        public long Open_less_15_days { get; set; }
    }
}