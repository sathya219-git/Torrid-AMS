namespace Incident.API.Dtos.Responses
{
    public class AssignmentGroupResponse
    {
        public string AssignmentGroupName { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
