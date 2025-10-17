namespace Incident.Domain.Models
{
    public class IncidentModel
    {
        public int Id { get; set;}
        public string Title { get; set;}
        public string Description { get; set;}
        public string CreatedDate { get; set;}
        public string ResolvedDate { get; set;}
        public string Status { get; set;}        
    }
}