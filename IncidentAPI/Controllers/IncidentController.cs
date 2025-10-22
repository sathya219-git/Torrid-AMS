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
                NewIncidents = result.NewIncidents,
                OpenIncidents = result.OpenIncidents,
                InProgressIncidents = result.InProgressIncidents,
                OnHoldIncidents = result.OnHoldIncidents,
                ResolvedIncidents = result.ResolvedIncidents,
                ClosedIncidents = result.ClosedIncidents
            };

            return Ok(response);
        }

        [HttpGet("nameandcountbypriority")]
        public async Task<IActionResult> GetNameAndIncidentCountByPriority([FromQuery] DashboardFilterRequest request)
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

            var result = await _incidentService.GetNameAndIncidentCountByPriorityAsync(filter);

            var response = result.Select(d => new NameAndIncidentCountByPriorityResponse
            {
                AssignedToName = d.AssignedToName,
                Priority = d.Priority,
                IncidentCount = d.IncidentCount
            });

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

            var domainResult = await _incidentService.GetAssignmentGroupsAsync(filter);

            var response = domainResult.Select(d => new AssignmentGroupResponse
            {
                AssignmentGroupName = d.AssignmentGroupName
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
                AssignedToName = request.AssignedToName
            };

            var result = await _incidentService.GetStatusCountByPriorityAsync(filter);
            var response = result.Select(d => new StatusCountByPriorityResponse
            {
                Priority = d.Priority,
                Status = d.Status,
                IncidentCount = d.IncidentCount
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

            var domainResult = await _incidentService.GetCategoryCountByGroupAsync(filter);

            var response = domainResult.Select(d => new CategoryCountByGroupResponse
            {
                CategoryName = d.CategoryName,
                IncidentCount = d.IncidentCount
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
                AssignedToName = request.AssignedToName
            };

            if (filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate > filter.ToDate)
                return BadRequest("FromDate cannot be later than ToDate.");

            var allowedPriorities = new List<string> { "1 - Critical", "2 - High", "3 - Moderate", "4 - Low" };
            if (!string.IsNullOrEmpty(filter.Priority) && !allowedPriorities.Contains(filter.Priority))
                return BadRequest("Invalid priority value.");

            try
            {
                var result = await _incidentService.GetIncidentCountByPriorityAsync(filter);

                if (result == null || !result.Any())
                {
                    return NotFound("No incidents found for the given criteria.");
                }

                var response = result.Select(d => new IncidentCountByPriorityResponse
                {
                    Priority = d.Priority,
                    IncidentCount = d.IncidentCount
                });

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("detailsbypriority")]
        public async Task<IActionResult> GetIncidentDetailsByPriority([FromQuery] DashboardFilterRequest request)
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

            if (filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate > filter.ToDate)
                return BadRequest("FromDate cannot be later than ToDate.");
                
            var allowedPriorities = new List<string> { "1 - Critical", "2 - High", "3 - Moderate", "4 - Low" };
            if (!string.IsNullOrEmpty(filter.Priority) && !allowedPriorities.Contains(filter.Priority))
                return BadRequest("Invalid priority value.");
            
            try
            {
                var result = await _incidentService.GetIncidentDetailsByPriorityAsync(filter);

                if (result == null || !result.Any())
                {
                    return NotFound("No incidents found for the given criteria.");
                }

                var response = result.Select(d => new IncidentDetailsResponse
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
                LastUpdated = d.LastUpdated,
                UpdatedBy = d.UpdatedBy,
                ChildIncidents = d.ChildIncidents,
                SLADueDate = d.SLADueDate,
                SeverityLevel = d.SeverityLevel,
                SubcategoryName = d.SubcategoryName,
                ResolutionTime_Hours = d.ResolutionTime_Hours
                });

                return Ok(response);
            }
            catch (Exception)
            {             
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
    
}