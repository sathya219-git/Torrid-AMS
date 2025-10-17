using Incident.Domain.Models;

namespace Incident.Application.Interfaces
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<IncidentModel>> GetAllAsync();
    }
}