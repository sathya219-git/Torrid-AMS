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

        public async Task<IEnumerable<NameAndIncidentCountByPriority>> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching incident count by priority");

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
            try
            {
                var result = await _incidentRepository.GetDashboardKpisAsync(filter);

                if (result == null)
                    _logger.LogWarning("No dashboard KPIs found for filter: {@Filter}", filter);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard KPIs");
                throw;
            }
        }
        public async Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching status count by priority");
            return await _incidentRepository.GetStatusCountByPriorityAsync(filter);
        }

        public async Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching category count by group");
            return await _incidentRepository.GetCategoryCountByGroupAsync(filter);
        }
        public async Task<IEnumerable<IncidentCountByPriority>> GetIncidentCountByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching incident count by priority");
            return await _incidentRepository.GetIncidentCountByPriorityAsync(filter);
        }

        public async Task<IEnumerable<IncidentDetails>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter)
        {
            _logger.LogInformation("Fetching incident details by priority");
            return await _incidentRepository.GetIncidentDetailsByPriorityAsync(filter);
        }       

    }
}
