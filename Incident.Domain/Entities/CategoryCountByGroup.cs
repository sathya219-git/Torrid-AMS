namespace Incident.Domain.Entities
{
    public class CategoryCountByGroup
    {
        public string CategoryName { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }
}
