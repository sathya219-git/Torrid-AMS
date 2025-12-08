namespace Incident.Application.Dtos.Responses
{
    public class StatusCountByPriorityResponse
    {
        public string Status { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}

    