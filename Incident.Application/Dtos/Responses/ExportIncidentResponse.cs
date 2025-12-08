namespace Incident.Application.Dtos.Responses
{
    public class ExportIncidentResponse
    {
  public string Number { get; set; } = string.Empty;
        public DateTime? Opened { get; set; }
        public string Short_Description { get; set; } = string.Empty;
        public string Caller { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Assignment_Group { get; set; } = string.Empty;
        public string Assigned_To { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public string Updated_By { get; set; } = string.Empty;
        public int? Child_Incidents { get; set; }
        public DateTime? Sla_Due { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Subcategory { get; set; } = string.Empty;
        public string Resolution_Notes { get; set; } = string.Empty;
        public DateTime? Resolved { get; set; }
        public string Sla_Calculation { get; set; } = string.Empty;
        public string Parent_Incident { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
        public string Task_Type { get; set; } = string.Empty;
    }
}
