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
                AssignedToName = request.AssignedToName
            };

            var result = await _incidentService.GetDashboardKpisAsync(filter);

            var response = new DashboardKpiResponse
            {
                TotalIncidents = result?.TotalIncidents ?? 0,
                OpenIncidents = result?.OpenIncidents ?? 0,
                InProgressIncidents = result?.InProgressIncidents ?? 0,
                ClosedIncidents = result?.ClosedIncidents ?? 0
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
                Metrics = request.Metrics,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SortBy = request.SortBy
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
                    AvgResolvedTime = x.AvgResolvedTime
                }).ToList() ?? new List<NameAndIncidentCountByPriorityResponse>(),

                Pagination = new PaginationResponse
                {
                    Page = result?.Pagination?.Page ?? 1,
                    PageSize = result?.Pagination?.PageSize ?? 0,
                    TotalRecords = result?.Pagination?.TotalRecords ?? 0,
                    TotalPages = result?.Pagination?.TotalPages ?? 0,
                    SortBy = result?.Pagination?.SortBy ?? string.Empty
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
                State = request.State,
                Metrics = request.Metrics
            };

            var result = await _incidentService.GetIncidentCountByPriorityAsync(filter);
            var response = new IncidentCountByPriorityGroupedResponse();

            if (result == null || !result.Any())
            {
                response.Priority = new Dictionary<string, List<IncidentStateCount>>();
                response.TotalAverageResolvedTime = "0";
                return Ok(response);
            }

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

                response.Priority[group.Key] = new List<IncidentStateCount> { stateCount };
                response.TotalAverageResolvedTime = first.TotalAverageResolvedTime;
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

            if (result == null || !result.Any())
            {
                var emptyResponse = new IncidentDetailsByPriorityPagedResponse
                {
                    PageNumber = 1,
                    PageSize = 8,
                    TotalPages = 0,
                    TotalElements = 0,
                    Incidents = new List<IncidentDetailsByPriorityItemResponse>
                    {
                        new IncidentDetailsByPriorityItemResponse()
                    }
                };

                return Ok(emptyResponse);
            }

            var first = result.First();

            var response = new IncidentDetailsByPriorityPagedResponse
            {
                PageNumber = first.PageNumber,
                PageSize = first.PageSize,
                TotalPages = first.TotalPages,
                TotalElements = first.TotalElements,
                Incidents = result.Select(r => new IncidentDetailsByPriorityItemResponse
                {
                    IncidentNo = r.IncidentNo,
                    Description = r.Description,
                    Category = r.Category,
                    ResolutionNotes = r.ResolutionNotes,
                    State = r.State,
                    ResolvedDateTime = r.ResolvedDateTime
                }).ToList()
            };

            return Ok(response);
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
                ShortDescription = r.ShortDescription,
                Caller = r.Caller,
                Priority = r.Priority,
                State = r.State,
                Category = r.Category,
                AssignmentGroup = r.AssignmentGroup,
                AssignedTo = r.AssignedTo,
                Updated = r.Updated,
                UpdatedBy = r.UpdatedBy,
                ChildIncidents = r.ChildIncidents,
                SlaDue = r.SlaDue,
                Severity = r.Severity,
                Subcategory = r.Subcategory,
                ResolutionNotes = r.ResolutionNotes,
                Resolved = r.Resolved,
                SlaCalculation = r.SlaCalculation,
                ParentIncident = r.ParentIncident,
                Parent = r.Parent,
                TaskType = r.TaskType
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
    }
    
}