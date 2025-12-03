using Dapper;
using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Incident.Application.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql; 
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Incident.Application.Helpers;
using System.Linq;

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
            using var connection = new NpgsqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            parameters.Add("@p_category", filter.Category.ToCsv());
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("@p_priority", filter.Priority.ToCsv());
            parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
            parameters.Add("@p_state", filter.State.ToCsv());
            parameters.Add("@p_pageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@p_pageSize", filter.PageSize < 0 ? 4 : filter.PageSize);
            parameters.Add("@p_sortBy", string.IsNullOrWhiteSpace(filter.SortBy) ? "Name" : filter.SortBy);
            parameters.Add("@p_sortOrder", string.IsNullOrWhiteSpace(filter.SortBy) ? "ASC" : filter.SortOrder);

            await connection.OpenAsync();
            var results = await connection.QueryAsync<dynamic>(
                "SELECT * FROM \"sp_nameandincidentcountbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_priority, @p_assignedToName, @p_state, @p_pageNumber, @p_pageSize, @p_sortBy, @p_sortOrder)",
                parameters
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
                        SortBy = filter.SortBy ?? "Name",
                        SortOrder = filter.SortOrder ?? "ASC"
                    }
                };
            }

            var first = results.First();

            var memberList = results.Select(r => new NameAndIncidentCountByPriority
            {
                Name = (string)r.name, 
                P1 = (int)r.p1,
                P2 = (int)r.p2,
                P3 = (int)r.p3,
                P4 = (int)r.p4,
                TotalCount = (int)r.totalcount,
                ActualResolvedTime = (string)r.actualresolvedtime,
                LastUpdated = r.lastupdated 
            }).ToList();

            var pagination = new PaginationInfo
            {
                Page = (int)(first.currentpage ?? filter.PageNumber),
                PageSize = (int)(first.pagesize ?? filter.PageSize),
                TotalRecords = (int)(first.totalrecords ?? 0), 
                TotalPages = (int)Math.Ceiling((decimal)(first.totalpages ?? 0)),
                SortBy = (string)(first.name ?? filter.SortBy ?? "Name"), 
                SortOrder = (string)(first.sortorder ?? filter.SortOrder ?? "ASC") 
            };

            return new PagedMemberIncidentStats
            {
                MemberDetails = memberList,
                Pagination = pagination
            };
        }

        public async Task<IEnumerable<IncidentCountByPriority>> GetIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_incidentcountbypriority' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
                parameters.Add("@p_state", filter.State.ToCsv());

                await connection.OpenAsync();
                var result = await connection.QueryAsync<IncidentCountByPriority>(
                    "SELECT * FROM \"sp_incidentcountbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_priority, @p_assignedToName, @p_state)",
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing function 'sp_incidentcountbypriority'");
                throw;
            }
        }

        public async Task<IEnumerable<BreachListItem>> GetBreachListByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Executing function 'sp_breachlistbypriority' with parameters: {@Filter}", filter);
            using var connection = new NpgsqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            parameters.Add("@p_category", filter.Category.ToCsv());
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
            parameters.Add("@p_state", filter.State.ToCsv());
            parameters.Add("@p_search", filter.Search);
            parameters.Add("@p_priority", filter.Priority.ToCsv());
            parameters.Add("@p_pageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@p_pageSize", filter.PageSize <= 0 ? 8 : filter.PageSize);
            parameters.Add("@p_sortBy", string.IsNullOrEmpty(filter.SortBy) ? "Updated" : filter.SortBy);
            parameters.Add("@p_sortOrder", string.IsNullOrEmpty(filter.SortOrder) ? "DESC" : filter.SortOrder);
            parameters.Add("@p_incidentNumber", filter.IncidentNumber.ToCsv());
            parameters.Add("@p_actualResolvedTime", filter.ActualResolvedTime.ToCsv());
            parameters.Add("@p_breachSLA", filter.BreachSLA.ToCsv());

            try
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<BreachListItem>(
                    "SELECT * FROM \"sp_breachlistbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_assignedToName, @p_state, @p_search, @p_priority, @p_pageNumber, @p_pageSize, @p_sortBy, @p_sortOrder, @p_incidentNumber, @p_actualResolvedTime, @p_breachSLA)",
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                // 🎯 CORRECTION: Update logger message
                _logger.LogError(ex, "Error executing function 'sp_breachlistbypriority'");
                throw;
            }
        } 
        
        public async Task<IEnumerable<IncidentDetailsByPriority>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Executing function 'sp_getincidentdetailsbypriority' with parameters: {@Filter}", filter);
            using var connection = new NpgsqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            parameters.Add("@p_category", filter.Category.ToCsv());
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
            parameters.Add("@p_state", filter.State.ToCsv());
            parameters.Add("@p_search", filter.Search);
            parameters.Add("@p_priority", filter.Priority.ToCsv());
            parameters.Add("@p_pageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@p_pageSize", filter.PageSize <= 0 ? 8 : filter.PageSize);
            parameters.Add("@p_sortBy", string.IsNullOrEmpty(filter.SortBy) ? "Resolved" : filter.SortBy);
            parameters.Add("@p_sortOrder", string.IsNullOrEmpty(filter.SortOrder) ? "DESC" : filter.SortOrder);

            try
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<IncidentDetailsByPriority>(
                    "SELECT * FROM \"sp_getincidentdetailsbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_assignedToName, @p_state, @p_search, @p_priority, @p_pageNumber, @p_pageSize, @p_sortBy, @p_sortOrder)",
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing function 'sp_getincidentdetailsbypriority'");
                throw;
            }
        }
        public async Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_GetAssignmentGroups' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
                parameters.Add("@p_state", filter.State.ToCsv());

                await connection.OpenAsync();
                var result = await connection.QueryAsync<AssignmentGroup>(
                    "SELECT * FROM \"sp_getassignmentgroups\"(@p_fromDate, @p_toDate, @p_category, @p_priority, @p_assignedToName)", // Presuming sp_GetAssignmentGroups exists
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing function 'sp_GetAssignmentGroups'");
                throw;
            }
        }
        public async Task<DashboardKpi?> GetDashboardKpisAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_GetDashboardKpis' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var parameters = new DynamicParameters();
                
                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
                parameters.Add("@p_state", filter.State.ToCsv());
            
                await connection.OpenAsync();
                
                var result = await connection.QueryFirstOrDefaultAsync<DashboardKpi>(
                    "SELECT * FROM \"sp_getdashboardkpis\"(@p_fromDate, @p_toDate, @p_assignmentGroup, @p_category, @p_priority, @p_assignedToName, @p_state)", // Presuming sp_GetDashboardKpis exists
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing function 'sp_GetDashboardKpis'");
                throw;
            }
        }
        public async Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_StatusCountByPriority' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
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
                    "SELECT * FROM \"sp_statuscountbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_priority, @p_assignedToName, @p_state)", // Presuming sp_StatusCountByPriority exists
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing function 'sp_StatusCountByPriority'");
                throw;
            }
        }
        public async Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_GetCategoryCountByGroup' with parameters: {@Filter}", filter);

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());

                await connection.OpenAsync();
                var result = await connection.QueryAsync<CategoryCountByGroup>(
                    "SELECT * FROM \"sp_getcategorycountbygroup\"(@p_fromDate, @p_toDate, @p_assignmentGroup)", // Presuming sp_GetCategoryCountByGroup exists
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing function 'sp_GetCategoryCountByGroup'");
                throw;
            }
        }
        public async Task<IEnumerable<ExportIncident>> ExportIncidentsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_ExportIncidents' with parameters: {@Filter}", filter);
            using var connection = new NpgsqlConnection(_connectionString);

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
                    "SELECT * FROM \"sp_exportincidents\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_priority, @p_assignedToName, @p_state)", // Presuming sp_ExportIncidents exists
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing function 'sp_ExportIncidents'");
                throw;
            }
        }
    }
}