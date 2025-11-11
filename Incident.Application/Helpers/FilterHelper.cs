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
        public static string FormatSize(long? bytes)
        {
            if (bytes == null || bytes < 1024) return $"{bytes ?? 0} B";
            double size = bytes.Value;
            string[] units = { "B", "KB", "MB", "GB" };
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return $"{size:0.#} {units[unit]}";
        }
    }
}
