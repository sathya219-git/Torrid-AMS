using Incident.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Incident.Application.Models;

namespace Incident.Application.Interfaces
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<NameAndIncidentCountByPriority>> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter);
        Task<DashboardKpi?> GetDashboardKpisAsync(IncidentFilter filter);
        Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter);
        Task<IEnumerable<IncidentCountByPriority>> GetIncidentCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<IncidentDetails>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter);


    }

}
