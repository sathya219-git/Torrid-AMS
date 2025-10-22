namespace Incident.API.Dtos.Responses
{
    public class StatusCountByPriorityResponse
    {
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
