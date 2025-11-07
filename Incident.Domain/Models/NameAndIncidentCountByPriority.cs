namespace Incident.Domain.Models
{
    public class NameAndIncidentCountByPriority
    {
        public string Name { get; set; } = string.Empty;
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public int P4 { get; set; }
        public int TotalCount { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string ActualResolvedTime { get; set; } = string.Empty;

    }
        public class PagedMemberIncidentStats
    {
        public IEnumerable<NameAndIncidentCountByPriority> MemberDetails { get; set; } = new List<NameAndIncidentCountByPriority>();
        public PaginationInfo Pagination { get; set; } = new();
    }

    public class PaginationInfo
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public string SortOrder{ get; set; } = string.Empty;
    }
}
