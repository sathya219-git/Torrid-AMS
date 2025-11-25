using Incident.API.Dtos.Requests;
using Incident.API.Dtos.Responses;
using Incident.Application.Interfaces;
using Incident.Application.Models;
using Incident.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
 
namespace IncidentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> GetDashboardKpis([FromQuery] DashboardFilterRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                AssignmentGroup = request.AssignmentGroup,
                Category = request.Category,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName,
                State = request.State
            };

            var result = await _incidentService.GetDashboardKpisAsync(filter);

            var response = new DashboardKpiResponse
            {
                TotalIncidents = result?.TotalIncidents ?? 0,
                OpenIncidents = result?.OpenIncidents ?? 0,
                InProgressIncidents = result?.InProgressIncidents ?? 0,
                ClosedIncidents = result?.ClosedIncidents ?? 0,
                BreachedCount = result?.Breached ?? 0
            };

            return Ok(response);
        }

        [HttpGet("nameandcountbypriority")]
        public async Task<IActionResult> GetNameAndIncidentCountByPriority([FromQuery] DashboardFilterPaginatedRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                AssignmentGroup = request.AssignmentGroup,
                Category = request.Category,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName,
                State = request.State,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder
            };

            var result = await _incidentService.GetNameAndIncidentCountByPriorityAsync(filter);

            var response = new MemberIncidentStatsResponse
            {
                MemberDetails = result?.MemberDetails?.Select(x => new NameAndIncidentCountByPriorityResponse
                {
                    Name = x.Name,
                    P1 = x.P1,
                    P2 = x.P2,
                    P3 = x.P3,
                    P4 = x.P4,
                    TotalCount = x.TotalCount,
                    ActualResolvedTime = x.ActualResolvedTime,
                    LastUpdated = x.LastUpdated
                }).ToList() ?? new List<NameAndIncidentCountByPriorityResponse>(),

                Pagination = new PaginationResponse
                {
                    Page = result?.Pagination?.Page ?? 1,
                    PageSize = result?.Pagination?.PageSize ?? 0,
                    TotalRecords = result?.Pagination?.TotalRecords ?? 0,
                    TotalPages = result?.Pagination?.TotalPages ?? 0,
                    SortBy = result?.Pagination?.SortBy ?? string.Empty,
                    SortOrder = result?.Pagination?.SortOrder ?? string.Empty
                }
            };

            return Ok(response);
        }


        [HttpGet("assignmentgroups")]
        public async Task<IActionResult> GetAssignmentGroups([FromQuery] DashboardFilterRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Category = request.Category,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName
            };

            var result = await _incidentService.GetAssignmentGroupsAsync(filter);

            var response = (result == null || !result.Any())
                ? new List<AssignmentGroupResponse>() 
                : result.Select(r => new AssignmentGroupResponse
                {
                    AssignmentGroupName = r.AssignmentGroupName
                }).ToList();

            return Ok(response);
        }


        [HttpGet("statuscountbypriority")]
        public async Task<IActionResult> GetStatusCountByPriority([FromQuery] DashboardFilterRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                AssignmentGroup = request.AssignmentGroup,
                Category = request.Category,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName,
                State = request.State
            };

            var result = await _incidentService.GetStatusCountByPriorityAsync(filter);

            var response = (result == null || !result.Any())
                ? new List<StatusCountByPriorityResponse>()
                : result.Select(r => new StatusCountByPriorityResponse
                {
                    Status = r.Status,
                    IncidentCount = r.IncidentCount
                }).ToList();

            return Ok(response);
        }

        [HttpGet("categorycountbygroup")]
        public async Task<IActionResult> GetCategoryCountByGroup([FromQuery] DashboardFilterRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                AssignmentGroup = request.AssignmentGroup
            };

            var result = await _incidentService.GetCategoryCountByGroupAsync(filter);

            var response = (result == null || !result.Any())
                ? new List<CategoryCountByGroupResponse>() 
                : result.Select(r => new CategoryCountByGroupResponse
                {
                    CategoryName = r.CategoryName,
                    IncidentCount = r.IncidentCount
                }).ToList();

            return Ok(response);
        }

        [HttpGet("countbypriority")]
             public async Task<IActionResult> GetIncidentCountByPriority([FromQuery] DashboardFilterRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                AssignmentGroup = request.AssignmentGroup,
                Category = request.Category,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName,
                State = request.State
            };
        
            var result = await _incidentService.GetIncidentCountByPriorityAsync(filter);
            var response = new IncidentCountByPriorityGroupedResponse();        
            if (result == null || !result.Any())
                return Ok(response);        
            foreach (var group in result.GroupBy(r => r.Priority))
            {
                var first = group.First();        
                var stateCount = new IncidentStateCount
                {
                    TotalCount = first.TotalCount,
                    Open = group.FirstOrDefault(g => g.State.Equals("Open", StringComparison.OrdinalIgnoreCase))?.IncidentCount ?? 0,
                    InProgress = group.FirstOrDefault(g => g.State.Equals("In Progress", StringComparison.OrdinalIgnoreCase))?.IncidentCount ?? 0,
                    Closed = group.FirstOrDefault(g => g.State.Equals("Closed", StringComparison.OrdinalIgnoreCase))?.IncidentCount ?? 0,
                    OnHold = group.FirstOrDefault(g => g.State.Equals("On Hold", StringComparison.OrdinalIgnoreCase))?.IncidentCount ?? 0,
                    Reopen = group.FirstOrDefault(g => g.State.Equals("Reopen", StringComparison.OrdinalIgnoreCase))?.IncidentCount ?? 0,
                    Resolved = group.FirstOrDefault(g => g.State.Equals("Resolved", StringComparison.OrdinalIgnoreCase))?.IncidentCount ?? 0
                };

                var totalBreachedForPriority = group.Sum(g => g.BreachedCount);        
                response.Priority[group.Key] = new PriorityData
                {
                    Details = new List<IncidentStateCount> { stateCount },
                    AvgResolvedTime = first.AvgResolvedTime,
                    TotalResolvedTime = first.TotalResolvedTime,
                    BreachedCount = totalBreachedForPriority
                };
            }        
            return Ok(response);
        }

        [HttpGet("detailsbypriority")]
        public async Task<IActionResult> GetIncidentDetailsByPriority([FromQuery] DashboardFilterPaginatedRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Category = request.Category,
                AssignmentGroup = request.AssignmentGroup,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName,
                State = request.State,
                Search = request.Search,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _incidentService.GetIncidentDetailsByPriorityAsync(filter);

            var paginatedResponse = new IncidentDetailsPaginatedResponse
            {
                PageNumber = result.FirstOrDefault()?.PageNumber ?? 1,
                PageSize = result.FirstOrDefault()?.PageSize ?? 8,
                TotalPages = result.FirstOrDefault()?.TotalPages ?? 0,
                TotalElements = result.FirstOrDefault()?.TotalElements ?? 0,
                Incidents = result.Select(x => new IncidentDetailsResponse
                {
                    IncidentNo = x.IncidentNo,
                    AssignedTo = x.AssignedTo,
                    ShortDescription = x.ShortDescription,
                    Category = x.Category,
                    State = x.State,
                    CreatedDateTime = x.Created,
                    UpdatedDateTime = x.Updated,
                    ResolvedDateTime = x.ResolvedDateTime,
                    ActualResolvedTime = x.ActualResolvedTime,
                    BreachSLA = x.BreachSLA
                }).ToList()
            };

            if (result == null || !result.Any())
            {
                paginatedResponse.Incidents = new List<IncidentDetailsResponse>();
            }

            return Ok(paginatedResponse);
        }



        [HttpGet("export")]
        public async Task<IActionResult> ExportIncidents([FromQuery] DashboardFilterRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                AssignmentGroup = request.AssignmentGroup,
                Category = request.Category,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName,
                State = request.State
            };

            var incidents = await _incidentService.ExportIncidentsAsync(filter);

            var responseDto = incidents.Select(r => new ExportIncidentResponse
            {
                Number = r.Number,
                Opened = r.Opened,
                Short_Description = r.Short_Description,
                Caller = r.Caller,
                Priority = r.Priority,
                State = r.State,
                Category = r.Category,
                Assignment_Group = r.Assignment_Group,
                Assigned_To = r.Assigned_To,
                Updated = r.Updated,
                Updated_By = r.Updated_By,
                Child_Incidents = r.Child_Incidents,
                Sla_Due = r.Sla_Due,
                Severity = r.Severity,
                Subcategory = r.Subcategory,
                Resolution_Notes = r.Resolution_Notes,
                Resolved = r.Resolved,
                Sla_Calculation = r.Sla_Calculation,
                Parent_Incident = r.Parent_Incident,
                Parent = r.Parent,
                Task_Type = r.Task_Type
            });

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Incidents");

            var properties = typeof(ExportIncidentResponse).GetProperties();
            for (int i = 0; i < properties.Length; i++)
                worksheet.Cell(1, i + 1).Value = properties[i].Name;

            int row = 2;
            foreach (var incident in responseDto)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var cellValue = properties[col].GetValue(incident);
                    worksheet.Cell(row, col + 1).SetValue(cellValue?.ToString() ?? string.Empty);
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"IncidentsExport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        
        [HttpGet("breachlistbypriority")]
        public async Task<IActionResult> GetBreachListByPriority([FromQuery] DashboardFilterPaginatedRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Category = request.Category,
                AssignmentGroup = request.AssignmentGroup,
                AssignedToName = request.AssignedToName,
                State = request.State,
                Search = request.Search,
                Priority = request.Priority,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder,
                IncidentNumber = request.IncidentNumber,
                ActualResolvedTime = request.ActualResolvedTime,
                BreachSLA = request.BreachSLA
            };

            var result = await _incidentService.GetBreachListByPriorityAsync(filter);

            var response = new BreachListPagedResponse
            {
                PageNumber =result.FirstOrDefault()?.PageNumber ?? 1,
                PageSize = result.FirstOrDefault()?.PageSize ?? 8,
                TotalPages = result.FirstOrDefault()?.TotalPages ?? 0,
                TotalElements = result.FirstOrDefault()?.TotalElements ?? 0,
                Items = result.Select(i => new BreachListItemResponse
                {
                    IncidentNumber = i.IncidentNumber,
                    AssignedTo = i.AssignedTo,
                    ShortDescription = i.ShortDescription,
                    Category = i.Category,
                    State = i.State,
                    CreatedDateTime = i.Created,
                    UpdatedDateTime= i.Updated,
                    ResolvedDateTime = i.ResolvedDateTime,
                    ActualResolvedTime = i.ActualResolvedTime,
                    BreachSLA = i.BreachSLA
                }).ToList() ?? new List<BreachListItemResponse>()
            };

            return Ok(response);
        }
    }
    
}