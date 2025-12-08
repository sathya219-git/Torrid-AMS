using Incident.Application.Dtos.Requests;
using Incident.Application.Interfaces;
using IncidentAPI.Mappers; 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Incident.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);
            if (user == null) return Unauthorized("Invalid credentials.");
            if (user.Message == "Login successful")
            {
                string role = (user.Username == "admin") ? "Admin" : "Standard";
                user.Role = role;
                
                var token = _tokenService.GenerateToken(user, role);

                return Ok(user.ToLoginResponse(token));
            }

            return Unauthorized(new { message = user.Message });
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> UpdatePasswordByDefault([FromBody] UpdatePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.UpdatePasswordByDefaultAsync(
                request.DefaultPassword,
                request.NewPassword,
                request.ConfirmNewPassword
            );

            // MAPPING: One line conversion
            var response = result.ToResponse();

            if (result.Success) return Ok(response);

            return BadRequest(response);
        }
    }
}