using Incident.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Incident.Application.Models;

namespace Incident.Application.Interfaces
{
    public interface IIncidentRepository
    {
        Task<PagedMemberIncidentStats> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter);
        Task<DashboardKpi?> GetDashboardKpisAsync(IncidentFilter filter);
        Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter);
        Task<IEnumerable<IncidentCountByPriority>> GetIncidentCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<IncidentDetailsByPriority>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<ExportIncident>> ExportIncidentsAsync(IncidentFilter filter);
        Task<BreachListPage> GetBreachListByPriorityAsync(IncidentFilter filter);

    }

}
