using Incident.Application.Interfaces;
using Incident.Domain.Models;

namespace Incident.Application.Services
{
    public class IncidentService : IIncidentService
    {
       private readonly IIncidentRepository _repository;

       public IncidentService(IIncidentRepository repository)
       {
         _repository = repository;
       }

       public async Task<IEnumerable<IncidentModel>> GetAllAsync()
        => await _repository.GetAllAsync();
    }
}