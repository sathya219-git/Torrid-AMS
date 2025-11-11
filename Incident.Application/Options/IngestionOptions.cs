namespace Incident.Application.Options
{
    public class IngestionOptions
    {
        public List<string> ExpectedHeaders { get; set; } = new();
        public bool RequireExactOrder { get; set; } = false;
    }
}