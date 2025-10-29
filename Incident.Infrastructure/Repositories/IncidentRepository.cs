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
using Incident.Application.Helpers;

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

        public async Task<PagedMemberIncidentStats> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            parameters.Add("@p_category", filter.Category.ToCsv());
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("@p_priority", filter.Priority.ToCsv());
            parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
            parameters.Add("@p_state", filter.State.ToCsv());
            parameters.Add("@p_metrics", filter.Metrics ?? "weeks");
            parameters.Add("@p_pageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@p_pageSize", filter.PageSize <= 0 ? 4 : filter.PageSize);
            parameters.Add("@p_sortBy", string.IsNullOrWhiteSpace(filter.SortBy) ? "Alphabetical" : filter.SortBy);

            await connection.OpenAsync();

            var results = await connection.QueryAsync<dynamic>(
                "sp_NameAndIncidentCountByPriority",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (!results.Any())
            {
                return new PagedMemberIncidentStats
                {
                    MemberDetails = new List<NameAndIncidentCountByPriority>(),
                    Pagination = new PaginationInfo
                    {
                        Page = filter.PageNumber,
                        PageSize = filter.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0,
                        SortBy = filter.SortBy ?? "Alphabetical"
                    }
                };
            }

            var first = results.First();

            var memberList = results.Select(r => new NameAndIncidentCountByPriority
            {
                Name = r.Name,
                P1 = (int)r.P1,
                P2 = (int)r.P2,
                P3 = (int)r.P3,
                P4 = (int)r.P4,
                TotalCount = (int)r.TotalCount,   
                AvgResolvedTime = (string)r.AvgResolvedTime
            }).ToList();

            var pagination = new PaginationInfo
            {
                Page = (int)(first.CurrentPage ?? filter.PageNumber),
                PageSize = (int)(first.PageSize ?? filter.PageSize),
                TotalRecords = (int)(first.TotalRecords ?? 0),
                TotalPages = (int)(first.TotalPages ?? 0),
                SortBy = (string)(first.SortBy ?? filter.SortBy ?? "Alphabetical")
            };

            return new PagedMemberIncidentStats
            {
                MemberDetails = memberList,
                Pagination = pagination
            };
        }

        public async Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_GetAssignmentGroups' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());

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
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());

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
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
                parameters.Add("@p_state", filter.State.ToCsv());

                await connection.OpenAsync();
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

            try
            {
                using var connection = new SqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());

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

            try
            {
                using var connection = new SqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
                parameters.Add("@p_state", filter.State.ToCsv());
                parameters.Add("@p_metrics", filter.Metrics ?? "weeks");

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

        public async Task<IEnumerable<IncidentDetailsByPriority>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Executing stored procedure 'sp_GetIncidentDetailsByPriority' with parameters: {@Filter}", filter);

            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            parameters.Add("@p_category", filter.Category.ToCsv());
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("@p_priority", filter.Priority.ToCsv());
            parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
            parameters.Add("@p_state", filter.State.ToCsv());
            parameters.Add("@p_search", filter.Search);
            parameters.Add("@p_sortBy", filter.SortBy ?? "Updated");
            parameters.Add("@p_sortOrder", filter.SortOrder ?? "DESC");
            parameters.Add("@p_pageNumber", filter.PageNumber);
            parameters.Add("@p_pageSize", filter.PageSize);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<IncidentDetailsByPriority>(
                    "sp_GetIncidentDetailsByPriority",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure 'sp_GetIncidentDetailsByPriority'");
                throw;
            }
        }
    
        
        public async Task<IEnumerable<ExportIncident>> ExportIncidentsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling SP 'sp_ExportIncidents' with parameters: {@Filter}", filter);

            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("@p_category", filter.Category.ToCsv());
            parameters.Add("@p_priority", filter.Priority.ToCsv());
            parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
            parameters.Add("@p_state", filter.State.ToCsv());

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<ExportIncident>(
                    "sp_ExportIncidents",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP 'sp_ExportIncidents'");
                throw;
            }
        }
    }
}
