namespace Incident.API.Dtos.Responses
{
    public class IncidentCountByPriorityResponse
    {
        public string Priority { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
