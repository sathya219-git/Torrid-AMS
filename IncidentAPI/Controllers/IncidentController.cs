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

            if (result == null)
                return NotFound();

            var response = new DashboardKpiResponse
            {
                TotalIncidents = result.TotalIncidents,
                OpenIncidents = result.OpenIncidents,
                InProgressIncidents = result.InProgressIncidents,
                ClosedIncidents = result.ClosedIncidents
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

            if (result == null || !result.MemberDetails.Any())
                return NotFound("No data found for given filters.");

            var response = new MemberIncidentStatsResponse
            {
                MemberDetails = result.MemberDetails.Select(x => new NameAndIncidentCountByPriorityResponse
                {
                    Name = x.Name,
                    P1 = x.P1,
                    P2 = x.P2,
                    P3 = x.P3,
                    P4 = x.P4,
                    TotalCount = x.TotalCount,   
                    AvgResolvedTime = x.AvgResolvedTime
                }),
                Pagination = new PaginationResponse
                {
                    Page = result.Pagination.Page,
                    PageSize = result.Pagination.PageSize,
                    TotalRecords = result.Pagination.TotalRecords,
                    TotalPages = result.Pagination.TotalPages,
                    SortBy = result.Pagination.SortBy
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

            if (result == null || !result.Any())
                return NotFound();

            var response = result.Select(r => new AssignmentGroupResponse
            {
                AssignmentGroupName = r.AssignmentGroupName
            });

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

            if (result == null || !result.Any())
                return NotFound();

            var response = result.Select(r => new StatusCountByPriorityResponse
            {
                Status = r.Status,
                IncidentCount = r.IncidentCount
            });

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

            if (result == null || !result.Any())
                return NotFound();

            var response = result.Select(r => new CategoryCountByGroupResponse
            {
                CategoryName = r.CategoryName,
                IncidentCount = r.IncidentCount
            });

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

            if (result == null || !result.Any())
                return NotFound();

            var response = new IncidentCountByPriorityGroupedResponse();

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

            var domainResult = await _incidentService.GetIncidentDetailsByPriorityAsync(filter);

            if (!domainResult.Any())
                return Ok(new PaginatedResponse<IncidentDetailsByPriorityResponse>
                {
                    Data = new List<IncidentDetailsByPriorityResponse>(),
                    TotalCount = 0
                });

            var totalCount = domainResult.First().TotalCount;

            var response = new PaginatedResponse<IncidentDetailsByPriorityResponse>
            {
                Data = domainResult.Select(d => new IncidentDetailsByPriorityResponse
                {
                    IncidentNumber = d.IncidentNumber,
                    OpenedDate = d.OpenedDate,
                    Description = d.Description,
                    CallerName = d.CallerName,
                    PriorityLevel = d.PriorityLevel,
                    CurrentState = d.CurrentState,
                    CategoryName = d.CategoryName,
                    AssignmentGroup = d.AssignmentGroup,
                    AssignedTo = d.AssignedTo,
                    ResolutionDate = d.ResolutionDate,
                    LastUpdated = d.LastUpdated,
                    UpdatedBy = d.UpdatedBy,
                    ChildIncidents = d.ChildIncidents,
                    SLADueDate = d.SLADueDate,
                    SeverityLevel = d.SeverityLevel,
                    SubcategoryName = d.SubcategoryName
                }),
                TotalCount = totalCount
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