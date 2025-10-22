using Incident.API.Dtos.Requests;
using Incident.API.Dtos.Responses;
using Incident.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Incident.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Username, request.Password);

            if (result != null)
            {
                if (result.Message == "Login successful")
                    return Ok(new LoginResponse
                    {
                        Message = result.Message ?? "Login successful",
                        UserID = result.UserID,
                        Username = result.Username,
                        Email = result.Email,
                        Role = result.Role
                    });
            }
                return Unauthorized(result);
            
        }
    }
}
