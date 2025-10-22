namespace Incident.Domain.Models
{
    public class IncidentCountByPriority
    {
        public string Priority { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
