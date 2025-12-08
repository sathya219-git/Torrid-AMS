using Incident.Domain.Entities;

public interface ITokenService
{
    string GenerateToken(User user, string role);
}