namespace Incident.Domain.Entities
{
    public class DashboardKpi
    {
        // Fixed Metrics
        public int TotalIncidents { get; set; }
        public int Breached_Count { get; set; }
        public int Open_Count { get; set; }
        public int Open_More_15_Days { get; set; }
        public int Open_Less_15_Days { get; set; }
        public Dictionary<string, int> StateCounts { get; set; } = new Dictionary<string, int>();
    }
}