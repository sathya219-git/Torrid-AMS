using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Incident.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Incident.Application.Interfaces
{
    public interface IIncidentService 
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
