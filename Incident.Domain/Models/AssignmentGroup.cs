namespace Incident.Domain.Models
{
    public class AssignmentGroup
    {
        public string AssignmentGroupName { get; set; }  = string.Empty;
        public int IncidentCount { get; set; }
    }
}
