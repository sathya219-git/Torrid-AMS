public class DashboardKpiResponse
{
    public int TotalIncidents { get; set; }
    public int OpenCount { get; set; }
    public int BreachedCount { get; set; }
    public int OpenMore15Days { get; set; }
    public int OpenLess15Days { get; set; }
    public Dictionary<string, int> States { get; set; }
}