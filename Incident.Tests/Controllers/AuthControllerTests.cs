using IncidentAPI.Controllers;
using Incident.API.Dtos.Requests;
using Incident.API.Dtos.Responses;
using Incident.Application.Models;
using Incident.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Incident.Application.Services;
using Incident.Application.Interfaces;
using Incident.API.Controllers;

namespace Incident.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Login_ReturnsOk_WithUser()
        {
            var mockService = new Mock<IAuthService>();
            mockService.Setup(s => s.LoginAsync("john", "password"))
                       .ReturnsAsync(new User
                       {
                           UserID = 1,
                           Username = "john",
                           Email = "john@example.com",
                           Role = "Admin",
                           Message = "Login successful"
                       });

            var controller = new AuthController(mockService.Object);

            var request = new LoginRequest { Username = "john", Password = "password" };

            var result = await controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponse>(okResult.Value);

            Assert.Equal("Login successful", response.Message);
            Assert.Equal(1, response.UserID);
        }
    }
}