namespace Incident.Domain.Entities
{
    public class AssignmentGroup
    {
        public string AssignmentGroupName { get; set; }  = string.Empty;
        public int IncidentCount { get; set; }
    }
}
