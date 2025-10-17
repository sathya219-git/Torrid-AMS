using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Microsoft.AspNetCore.Mvc;
 
namespace IncidentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _service;
 
        public IncidentController(IIncidentService service)
        {
            _service = service;
        }
 
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    }
}