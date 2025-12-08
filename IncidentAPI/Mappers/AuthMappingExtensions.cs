using Incident.Application.Dtos.Responses;
using Incident.Domain.Entities;

namespace IncidentAPI.Mappers
{
    public static class AuthMappingExtensions
    {
        // Maps the Domain User + Generated Token -> LoginResponse
        public static LoginResponse ToLoginResponse(this User user, string token)
        {
            if (user == null) return new LoginResponse();

            return new LoginResponse
            {
                Message = user.Message ?? "Login successful",
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role, 
                Token = token
            };
        }

        // Maps the Password Update Result -> UpdatePasswordResponse
        public static UpdatePasswordResponse ToResponse(this PasswordUpdateResult result)
        {
            return new UpdatePasswordResponse
            {
                Success = result.Success,
                Message = result.Message
            };
        }
    }
}