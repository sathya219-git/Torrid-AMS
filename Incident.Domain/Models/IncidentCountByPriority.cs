namespace Incident.Domain.Models
{
    public class IncidentCountByPriority
    {
        public string Priority { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
        public int? AvgResolutionTime_Hours { get; set; }
    }
}
