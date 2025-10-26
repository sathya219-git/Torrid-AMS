namespace Incident.API.Dtos.Responses
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
    }
}

