namespace Incident.API.Dtos.Responses
{
    public class DashboardKpiResponse
    {
        public int TotalIncidents { get; set; }
        public int NewIncidents { get; set; }
        public int OpenIncidents { get; set; }
        public int InProgressIncidents { get; set; }
        public int OnHoldIncidents { get; set; }
        public int ResolvedIncidents { get; set; }
        public int ClosedIncidents { get; set; }
    }
}
