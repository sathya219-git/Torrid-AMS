namespace Incident.Application.Dtos.Responses
{
    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public int? UserID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
    }
}
