namespace Incident.Application.Helpers
{
    public static class FilterHelper
    {
        public static string? ToCsv(this List<string>? list)
        {
            return (list == null || !list.Any())
                ? null
                : string.Join(",", list);
        }
    }
}
