namespace Incident.Domain.Models
{
    public class NameAndIncidentCountByPriority
    {
        public string AssignedToName { get; set; }  = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
         public int? AvgResolutionTime_Hours { get; set; }
    }
}
