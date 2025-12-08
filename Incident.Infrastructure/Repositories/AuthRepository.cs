using Dapper;
using Incident.Application.Interfaces;
using Incident.Domain.Entities;
using Npgsql; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Threading.Tasks;
using System;

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
            using var connection = new NpgsqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@Email", Email);
            parameters.Add("@Password", password);
                await connection.OpenAsync();
                var result = await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM \"sp_login\"(@Email, @Password)",
                    parameters
                );                
                return result; 
        }
        
        public async Task<PasswordUpdateResult> UpdatePasswordByDefaultAsync(
            string defaultPassword,
            string newPassword,
            string confirmNewPassword)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@DefaultPassword", defaultPassword);
            parameters.Add("@NewPassword", newPassword);
            parameters.Add("@ConfirmNewPassword", confirmNewPassword);

                await connection.OpenAsync();            
                await connection.ExecuteAsync(
                    "SELECT \"sp_updatepasswordbydefault\"(@DefaultPassword, @NewPassword, @ConfirmNewPassword)",
                    parameters
                );
                return new PasswordUpdateResult
                {
                    Success = true,
                    Message = "Password updated successfully."
                };
        }
    }
}