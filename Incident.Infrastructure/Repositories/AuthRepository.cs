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

        public async Task<User?> LoginAsync(string Email, string password)
        {
            _logger.LogInformation("Attempting login for user {Email}", Email);
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@Email", Email);
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
        
        public async Task<PasswordUpdateResult> UpdatePasswordByDefaultAsync(
            string defaultPassword,
            string newPassword,
            string confirmNewPassword)
        {
            _logger.LogInformation("Attempting default password update");

            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@DefaultPassword", defaultPassword);
            parameters.Add("@NewPassword", newPassword);
            parameters.Add("@ConfirmNewPassword", confirmNewPassword);

            try
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    "dbo.SP_UpdatePasswordByDefault",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return new PasswordUpdateResult
                {
                    Success = true,
                    Message = "Password updated successfully."
                };
            }
            catch (SqlException ex)
            {
                _logger.LogWarning(ex, "Failed to update password");
                return new PasswordUpdateResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating password");
                return new PasswordUpdateResult
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the password."
                };
            }
        }
    }
}
