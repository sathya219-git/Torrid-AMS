namespace Incident.Domain.Models
{
    public class PasswordUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
