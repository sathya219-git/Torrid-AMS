using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Incident.Application.Dtos.Requests;
using Incident.Application.Helpers; 
using Incident.Application.Interfaces;
using Incident.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Incident.Infrastructure.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<IncidentRepository> _logger;

        public IncidentRepository(IConfiguration configuration, ILogger<IncidentRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string is missing.");
            _logger = logger;
        }

        public async Task<PagedMemberIncidentStats> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_fromDate", filter.FromDate);
            parameters.Add("@p_toDate", filter.ToDate);
            // NOTE: If your Postgres function expects text[], use filter.Category?.ToArray() instead of ToCsv()
            parameters.Add("@p_category", filter.Category.ToCsv());
            parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("@p_priority", filter.Priority.ToCsv());
            parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());
            parameters.Add("@p_state", filter.State.ToCsv());
            parameters.Add("@p_pageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@p_pageSize", filter.PageSize < 0 ? 4 : filter.PageSize);
            parameters.Add("@p_sortBy", string.IsNullOrWhiteSpace(filter.SortBy) ? "Name" : filter.SortBy);
            parameters.Add("@p_sortOrder", string.IsNullOrWhiteSpace(filter.SortOrder) ? "ASC" : filter.SortOrder);

            await connection.OpenAsync();

            // Using dynamic mapping requires careful casting
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
                // SAFE CASTING: Handle DBNull and BigInt (Postgres default for counts)
                Name = r.name?.ToString(),
                P1 = Convert.ToInt32(r.p1 ?? 0),
                P2 = Convert.ToInt32(r.p2 ?? 0),
                P3 = Convert.ToInt32(r.p3 ?? 0),
                P4 = Convert.ToInt32(r.p4 ?? 0),
                TotalCount = Convert.ToInt32(r.totalcount ?? 0), // FIXED: (int)r.totalcount would crash
                ActualResolvedTime = r.actualresolvedtime?.ToString(),
                LastUpdated = r.lastupdated
            }).ToList();

            var pagination = new PaginationInfo
            {
                Page = Convert.ToInt32(first.currentpage ?? filter.PageNumber),
                PageSize = Convert.ToInt32(first.pagesize ?? filter.PageSize),
                TotalRecords = Convert.ToInt32(first.totalrecords ?? 0),
                TotalPages = Convert.ToInt32(Math.Ceiling((decimal)(first.totalpages ?? 0))),
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
            _logger.LogInformation("Calling function 'sp_incidentcountbypriority'");

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = new DynamicParameters();

            // Ensure these match the order/names in the PG function
            parameters.Add("p_fromdate", filter.FromDate);
            parameters.Add("p_todate", filter.ToDate);
            parameters.Add("p_category", filter.Category.ToCsv());
            parameters.Add("p_assignmentgroup", filter.AssignmentGroup.ToCsv());
            parameters.Add("p_priority", filter.Priority.ToCsv());
            parameters.Add("p_assignedtoname", filter.AssignedToName.ToCsv());
            parameters.Add("p_state", filter.State.ToCsv());

            await connection.OpenAsync();

            return await connection.QueryAsync<IncidentCountByPriority>(
                "SELECT * FROM public.sp_incidentcountbypriority(@p_fromdate, @p_todate, @p_category, @p_assignmentgroup, @p_priority, @p_assignedtoname, @p_state)",
                parameters
            );
        }

        public async Task<IEnumerable<BreachListItem>> GetBreachListByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Executing function 'sp_breachlistbypriority'");
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

                await connection.OpenAsync();
                var result = await connection.QueryAsync<BreachListItem>(
                    "SELECT * FROM \"sp_breachlistbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_assignedToName, @p_state, @p_search, @p_priority, @p_pageNumber, @p_pageSize, @p_sortBy, @p_sortOrder, @p_incidentNumber, @p_actualResolvedTime, @p_breachSLA)",
                    parameters
                );

                return result;
        }

        public async Task<IEnumerable<IncidentDetailsByPriority>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Executing function 'sp_getincidentdetailsbypriority'");
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

                await connection.OpenAsync();
                var result = await connection.QueryAsync<IncidentDetailsByPriority>(
                    "SELECT * FROM \"sp_getincidentdetailsbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_assignedToName, @p_state, @p_search, @p_priority, @p_pageNumber, @p_pageSize, @p_sortBy, @p_sortOrder)",
                    parameters
                );

                return result;

        }

        public async Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_GetAssignmentGroups'");
                using var connection = new NpgsqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());
                parameters.Add("@p_category", filter.Category.ToCsv());
                parameters.Add("@p_state", filter.State.ToCsv());
                parameters.Add("@p_priority", filter.Priority.ToCsv());
                parameters.Add("@p_assignedToName", filter.AssignedToName.ToCsv());

                await connection.OpenAsync();
                var result = await connection.QueryAsync<AssignmentGroup>(
                    "SELECT * FROM \"sp_getassignmentgroups\"(@p_fromDate, @p_toDate, @p_assignmentGroup, @p_category, @p_state, @p_priority, @p_assignedToName)",
                    parameters
                );
                return result;
        }

        public async Task<DashboardKpi?> GetDashboardKpisAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_GetDashboardKpis'");
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
 
                var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT * FROM \"sp_getdashboardkpis\"(@p_fromDate, @p_toDate, @p_assignmentGroup, @p_category, @p_priority, @p_assignedToName, @p_state)",
                    parameters
                );
            if (row == null) return null;
 
            var kpi = new DashboardKpi
            {
                TotalIncidents = (int)row.totalincidents,
                Breached_Count = (int)row.breached_count,
                Open_Count = (int)row.open_count,
                Open_More_15_Days = (int)row.open_more_15_days,
                Open_Less_15_Days = (int)row.open_less_15_days
            };
 
            if (row.state_counts != null)
            {
                string jsonContent = row.state_counts.ToString(); 
                kpi.StateCounts = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(jsonContent)
                                  ?? new Dictionary<string, int>();
            } 
            return kpi;
 
        }

        public async Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_StatusCountByPriority'");
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
                    "SELECT * FROM \"sp_statuscountbypriority\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_priority, @p_assignedToName, @p_state)",
                    parameters
                );
                return result;

        }

        public async Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_GetCategoryCountByGroup'");

                using var connection = new NpgsqlConnection(_connectionString);
                var parameters = new DynamicParameters();

                parameters.Add("@p_fromDate", filter.FromDate);
                parameters.Add("@p_toDate", filter.ToDate);
                parameters.Add("@p_assignmentGroup", filter.AssignmentGroup.ToCsv());

                await connection.OpenAsync();
                var result = await connection.QueryAsync<CategoryCountByGroup>(
                    "SELECT * FROM \"sp_getcategorycountbygroup\"(@p_fromDate, @p_toDate, @p_assignmentGroup)",
                    parameters
                );
                return result;
        }

        public async Task<IEnumerable<ExportIncident>> ExportIncidentsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Calling function 'sp_ExportIncidents'");
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
                var result = await connection.QueryAsync<ExportIncident>(
                    "SELECT * FROM \"sp_exportincidents\"(@p_fromDate, @p_toDate, @p_category, @p_assignmentGroup, @p_priority, @p_assignedToName, @p_state)",
                    parameters
                );
                return result;
        }
    }
}