using Incident.Application.Dtos.Requests;
using Incident.Application.Interfaces;
using IncidentAPI.Mappers; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IncidentAPI.Controllers
{
    [Authorize]
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
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetDashboardKpisAsync(filter);
            return Ok(result.ToResponse());
        }

        [HttpGet("nameandcountbypriority")]
        public async Task<IActionResult> GetNameAndIncidentCountByPriority([FromQuery] DashboardFilterPaginatedRequest request)
        {
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetNameAndIncidentCountByPriorityAsync(filter);

            // Clean: Logic is now inside IncidentMappingExtensions.cs
            return Ok(result.ToResponse());
        }

        [HttpGet("assignmentgroups")]
        public async Task<IActionResult> GetAssignmentGroups([FromQuery] DashboardFilterRequest request)
        {
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetAssignmentGroupsAsync(filter);
            return Ok(result.ToResponse());
        }

        [HttpGet("statuscountbypriority")]
        public async Task<IActionResult> GetStatusCountByPriority([FromQuery] DashboardFilterRequest request)
        {
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetStatusCountByPriorityAsync(filter);
            return Ok(result.ToResponse());
        }

        [HttpGet("categorycountbygroup")]
        public async Task<IActionResult> GetCategoryCountByGroup([FromQuery] DashboardFilterRequest request)
        {
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetCategoryCountByGroupAsync(filter);
            return Ok(result.ToResponse());
        }

        [HttpGet("countbypriority")]
        public async Task<IActionResult> GetIncidentCountByPriority([FromQuery] DashboardFilterRequest request)
        {
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetIncidentCountByPriorityAsync(filter);

            // The complex grouping logic is now hidden in the mapper
            return Ok(result.ToResponse());
        }

        [HttpGet("detailsbypriority")]
        public async Task<IActionResult> GetIncidentDetailsByPriority([FromQuery] DashboardFilterPaginatedRequest request)
        {
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetIncidentDetailsByPriorityAsync(filter);
            return Ok(result.ToResponse());
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportIncidents([FromQuery] DashboardFilterRequest request)
        {
            var filter = request.ToDomainFilter();

            // The Service now handles the Excel generation (ClosedXML)
            var fileContent = await _incidentService.ExportIncidentsToExcelAsync(filter);

            string fileName = $"IncidentsExport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("breachlistbypriority")]
        public async Task<IActionResult> GetBreachListByPriority([FromQuery] DashboardFilterPaginatedRequest request)
        {
            var filter = request.ToDomainFilter();
            var result = await _incidentService.GetBreachListByPriorityAsync(filter);
            return Ok(result.ToResponse());
        }
    }
}