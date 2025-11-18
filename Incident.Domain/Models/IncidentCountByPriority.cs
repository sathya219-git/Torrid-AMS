namespace Incident.Domain.Models
{
    public class IncidentCountByPriority
    {
        public string Priority { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
        public int TotalCount { get; set; }
        public string AvgResolvedTime { get; set; } = string.Empty;
        public string TotalResolvedTime { get; set; } = string.Empty;
        public int BreachedCount {get; set;}
    }
}
