namespace Incident.API.Dtos.Responses
{
    public class NameAndIncidentCountByPriorityResponse
    {
        public string AssignedToName { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
