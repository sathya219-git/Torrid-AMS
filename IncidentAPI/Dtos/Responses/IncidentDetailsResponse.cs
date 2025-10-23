namespace Incident.API.Dtos.Responses
{
    public class IncidentDetailsResponse
    {
        public string IncidentNumber { get; set; } = string.Empty;
        public DateTime? OpenedDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CallerName { get; set; } = string.Empty;
        public string PriorityLevel { get; set; } = string.Empty;
        public string CurrentState { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string AssignmentGroup { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? LastUpdated { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public string ChildIncidents { get; set; } = string.Empty;
        public DateTime? SLADueDate { get; set; }
        public string SeverityLevel { get; set; } = string.Empty;
        public string SubcategoryName { get; set; } = string.Empty;
        
    }
}
