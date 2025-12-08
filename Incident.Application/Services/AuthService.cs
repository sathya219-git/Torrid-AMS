using Incident.Application.Interfaces;
using Incident.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Incident.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<User?> LoginAsync(string Email, string password)
        {
            _logger.LogInformation("Service: login attempt for {Email}", Email);

            var user = await _authRepository.LoginAsync(Email, password);

            return user;
        }
        public async Task<PasswordUpdateResult> UpdatePasswordByDefaultAsync(string defaultPassword, string newPassword, string confirmNewPassword)
        {
            _logger.LogInformation("Service: update password by default");
            return await _authRepository.UpdatePasswordByDefaultAsync(defaultPassword, newPassword, confirmNewPassword);
        }
    }
}
