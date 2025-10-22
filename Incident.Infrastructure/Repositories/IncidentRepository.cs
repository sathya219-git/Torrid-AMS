using Dapper;
using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Incident.Application.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Incident.Infrastructure.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<IncidentRepository> _logger;

        public IncidentRepository(IConfiguration configuration, ILogger<IncidentRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string is missing.");
            _logger = logger;
        }

         public async Task<IEnumerable<NameAndIncidentCountByPriority>> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_NameAndIncidentCountByPriority' with parameters: {@Filter}", filter);

            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate, DbType.DateTime);
            parameters.Add("@p_toDate", filter.ToDate, DbType.DateTime);
            parameters.Add("@p_category", filter.Category, DbType.String);
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup, DbType.String);
            parameters.Add("@p_priority", filter.Priority, DbType.String);
            parameters.Add("@p_assignedToName", filter.AssignedToName, DbType.String);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<NameAndIncidentCountByPriority>(
                    "sp_NameAndIncidentCountByPriority",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_NameAndIncidentCountByPriority'");
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_GetAssignmentGroups' with parameters: {@Filter}", filter);

            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate, DbType.DateTime);
            parameters.Add("@p_toDate", filter.ToDate, DbType.DateTime);
            parameters.Add("@p_category", filter.Category, DbType.String);
            parameters.Add("@p_priority", filter.Priority, DbType.String);
            parameters.Add("@p_assignedToName", filter.AssignedToName, DbType.String);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<AssignmentGroup>(
                    "sp_GetAssignmentGroups",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_GetAssignmentGroups'");
                throw;
            }
        }

        public async Task<DashboardKpi?> GetDashboardKpisAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_GetDashboardKpis' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                var parameters = new DynamicParameters();
                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup);
                parameters.Add("@p_category", filter.Category);
                parameters.Add("@p_priority", filter.Priority);
                parameters.Add("@p_assignedToName", filter.AssignedToName);

                await connection.OpenAsync();
                var result = await connection.QueryFirstOrDefaultAsync<DashboardKpi>(
                    "sp_GetDashboardKpis",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_GetDashboardKpis'");
                throw;
            }
        }
        public async Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_StatusCountByPriority' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                var parameters = new DynamicParameters();
                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_category", filter.Category);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup);
                parameters.Add("@p_priority", filter.Priority);
                parameters.Add("@p_assignedToName", filter.AssignedToName);

                var result = await connection.QueryAsync<StatusCountByPriority>(
                    "sp_StatusCountByPriority",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_StatusCountByPriority'");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_GetCategoryCountByGroup' with parameters: {@Filter}", filter);

            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<CategoryCountByGroup>(
                    "sp_GetCategoryCountByGroup",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_GetCategoryCountByGroup'");
                throw;
            }
        }

        public async Task<IEnumerable<IncidentCountByPriority>> GetIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_IncidentCountByPriority' with parameters: {@Filter}", filter);

            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate, DbType.DateTime);
            parameters.Add("@p_toDate", filter.ToDate, DbType.DateTime);
            parameters.Add("@p_category", filter.Category, DbType.String);
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup, DbType.String);
            parameters.Add("@p_priority", filter.Priority, DbType.String);
            parameters.Add("@p_assignedToName", filter.AssignedToName, DbType.String);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<IncidentCountByPriority>(
                    "sp_IncidentCountByPriority",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_IncidentCountByPriority'");
                throw;
            }
        }
        
        public async Task<IEnumerable<IncidentDetails>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_GetIncidentDetailsByPriority' with parameters: {@Filter}", filter);

            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate, DbType.DateTime);
            parameters.Add("@p_toDate", filter.ToDate, DbType.DateTime);
            parameters.Add("@p_category", filter.Category, DbType.String);
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup, DbType.String);
            parameters.Add("@p_priority", filter.Priority, DbType.String);
            parameters.Add("@p_assignedToName", filter.AssignedToName, DbType.String);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<IncidentDetails>(
                    "sp_GetIncidentDetailsByPriority",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_GetIncidentDetailsByPriority'");
                throw;
            }
        }
    }
}
