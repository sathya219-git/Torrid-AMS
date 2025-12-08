using ClosedXML.Excel;
using Incident.Application.Dtos.Requests;
using Incident.Application.Dtos.Responses; // Needed for the property names mapping
using Incident.Application.Interfaces;
using Incident.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Incident.Application.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ILogger<IncidentService> _logger;

        public IncidentService(IIncidentRepository incidentRepository, ILogger<IncidentService> logger)
        {
            _incidentRepository = incidentRepository;
            _logger = logger;
        }

        public async Task<PagedMemberIncidentStats> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching NameAndIncidentCountByPriority with filter: {@Filter}", filter);
            return await _incidentRepository.GetNameAndIncidentCountByPriorityAsync(filter);
        }

        public async Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching assignment groups");
            return await _incidentRepository.GetAssignmentGroupsAsync(filter);
        }

        public async Task<DashboardKpi?> GetDashboardKpisAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching dashboard KPIs with filter: {@Filter}", filter);
            var result = await _incidentRepository.GetDashboardKpisAsync(filter);
            if (result == null)
                _logger.LogWarning("No dashboard KPIs found for filter: {@Filter}", filter);
            return result;
        }

        public async Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching status count by priority with filter: {@Filter}", filter);
            return await _incidentRepository.GetStatusCountByPriorityAsync(filter);
        }

        public async Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching category count by group");
            return await _incidentRepository.GetCategoryCountByGroupAsync(filter);
        }

        public async Task<IEnumerable<IncidentCountByPriority>> GetIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching incident count by priority with filter: {@Filter}", filter);
            return await _incidentRepository.GetIncidentCountByPriorityAsync(filter);
        }

        public async Task<IEnumerable<IncidentDetailsByPriority>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching incident details by priority with filter: {@Filter}", filter);
            return await _incidentRepository.GetIncidentDetailsByPriorityAsync(filter);
        }

        public async Task<IEnumerable<BreachListItem>> GetBreachListByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching breach list by priority with filter: {@Filter}", filter);
            return await _incidentRepository.GetBreachListByPriorityAsync(filter);
        }

        // --- NEW LOGIC MOVED FROM CONTROLLER ---
        public async Task<byte[]> ExportIncidentsToExcelAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Generating Excel export for incidents");

            var incidents = await _incidentRepository.ExportIncidentsAsync(filter);

            // We map to the Response object here just to keep the column structure identical to what you had.
            // In a pure Clean Architecture, you might use a specific "ExportDto" instead.
            var dataToExport = incidents.Select(r => new ExportIncidentResponse
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
            }).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Incidents");

            // Reflection-based Header Generation
            var properties = typeof(ExportIncidentResponse).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
            }

            // Data Population
            int row = 2;
            foreach (var item in dataToExport)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var cellValue = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).SetValue(cellValue?.ToString() ?? string.Empty);
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}