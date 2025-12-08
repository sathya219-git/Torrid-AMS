namespace Incident.Domain.Entities
{
    public class StatusCountByPriority
    {
        public string Status { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
