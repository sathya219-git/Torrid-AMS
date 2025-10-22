namespace Incident.API.Dtos.Responses
{
    public class CategoryCountByGroupResponse
    {
        public string CategoryName { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
