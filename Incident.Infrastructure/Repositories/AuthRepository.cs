using Dapper;
using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Threading.Tasks;

namespace Incident.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(IConfiguration configuration, ILogger<AuthRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string is missing.");
            _logger = logger;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Attempting login for user {Username}", username);
            using var connection = new SqlConnection(_connectionString);
            
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username);
            parameters.Add("@Password", password);

            try
            {
                await connection.OpenAsync();
                
                var result = await connection.QueryFirstOrDefaultAsync<User>(
                    "sp_Login",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result ?? new User { Message = "Invalid username or password" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_Login'");
                throw;
            }
        }
    }
}
