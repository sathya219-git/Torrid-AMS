namespace Incident.API.Dtos.Responses
{
    public class DashboardKpiResponse
    {
        public int TotalIncidents { get; set; }
        public int OpenIncidents { get; set; }
        public int InProgressIncidents { get; set; }
        public int ClosedIncidents { get; set; }
    }
}
