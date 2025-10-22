using Incident.Application.Interfaces;
using Incident.Domain.Models;
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

        public async Task<User?> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Service: login attempt for {Username}", username);

            var user = await _authRepository.LoginAsync(username, password);

            return user;
        }
    }
}
