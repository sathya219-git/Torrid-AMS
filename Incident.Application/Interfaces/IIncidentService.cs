using Incident.Application.Dtos.Requests;
using Incident.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Incident.Application.Interfaces
{
    public interface IIncidentService
    {
        Task<PagedMemberIncidentStats> GetNameAndIncidentCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<AssignmentGroup>> GetAssignmentGroupsAsync(IncidentFilter filter);
        Task<DashboardKpi?> GetDashboardKpisAsync(IncidentFilter filter);
        Task<IEnumerable<StatusCountByPriority>> GetStatusCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<CategoryCountByGroup>> GetCategoryCountByGroupAsync(IncidentFilter filter);
        Task<IEnumerable<IncidentCountByPriority>> GetIncidentCountByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<IncidentDetailsByPriority>> GetIncidentDetailsByPriorityAsync(IncidentFilter filter);
        Task<IEnumerable<BreachListItem>> GetBreachListByPriorityAsync(IncidentFilter filter);

        // CHANGED: Now returns the file bytes directly
        Task<byte[]> ExportIncidentsToExcelAsync(IncidentFilter filter);
    }
}