using Incident.Domain.Models;

namespace Incident.Application.Interfaces
{
    public interface IIncidentService
    {
        Task<IEnumerable<IncidentModel>> GetAllAsync();
    }
}