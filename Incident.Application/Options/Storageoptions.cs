namespace Incident.Application.Options
{
    public class StorageOptions
    {
        public string CsvRoot { get; set; } = string.Empty;
        public long MaxUploadBytes { get; set; } = 10 * 1024 * 1024;
    }
}