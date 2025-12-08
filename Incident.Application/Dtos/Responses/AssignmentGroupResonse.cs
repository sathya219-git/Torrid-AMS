namespace Incident.Application.Dtos.Responses
{
    public class AssignmentGroupResponse
    {
        public string AssignmentGroupName { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
