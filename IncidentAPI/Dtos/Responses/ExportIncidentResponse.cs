namespace Incident.API.Dtos.Responses
{
    public class ExportIncidentResponse
    {
  public string Number { get; set; } = string.Empty;
        public DateTime? Opened { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        public string Caller { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string AssignmentGroup { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public int? ChildIncidents { get; set; }
        public DateTime? SlaDue { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Subcategory { get; set; } = string.Empty;
        public string ResolutionNotes { get; set; } = string.Empty;
        public DateTime? Resolved { get; set; }
        public string SlaCalculation { get; set; } = string.Empty;
        public string ParentIncident { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
    }
}
