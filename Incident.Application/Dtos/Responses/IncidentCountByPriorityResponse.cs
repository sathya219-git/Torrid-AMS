namespace Incident.Application.Dtos.Responses
{
    public class IncidentCountByPriorityResponse
    {
        public string Priority { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
        public int? AvgResolutionTime_Hours { get; set; }
    }
}
