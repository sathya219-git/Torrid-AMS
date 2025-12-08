namespace Incident.Domain.Entities
{
    public class BreachListItem
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public string IncidentNumber { get; set; } = string.Empty;   
        public string AssignedTo { get; set; } = string.Empty;   
        public string ShortDescription { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; 
        public string State { get; set; } = string.Empty;       
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? ResolvedDateTime { get; set; }
        public string ActualResolvedTime { get; set; } = string.Empty; 
        public string BreachSLA { get; set; } = string.Empty; 
    }
}
