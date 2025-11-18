using Incident.Application.Interfaces;
using Incident.Application.Models;
using Incident.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            var result = await _incidentRepository.GetNameAndIncidentCountByPriorityAsync(filter);
            return result;
        }

        public async Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching assignment groups");
            var result = await _incidentRepository.GetAssignmentGroupsAsync(filter);
            return result;
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


        public async Task<IEnumerable<ExportIncident>> ExportIncidentsAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching export incidents data");
            var result = await _incidentRepository.ExportIncidentsAsync(filter);
            return result;
        }
        public async Task<IEnumerable<BreachListItem>> GetBreachListByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching breach list by priority with filter: {@Filter}", filter);
            return await _incidentRepository.GetBreachListByPriorityAsync(filter);
        }
    }
}
