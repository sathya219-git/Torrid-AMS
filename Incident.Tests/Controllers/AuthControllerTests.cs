using Incident.API.Controllers;
using Incident.Application.Dtos.Requests;
using Incident.Application.Dtos.Responses;
using Incident.Application.Interfaces;
using Incident.Domain.Entities; 
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Incident.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockTokenService = new Mock<ITokenService>();
            _controller = new AuthController(_mockAuthService.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@test.com", Password = "password" };
            var user = new User { Username = "admin", Email = "test@test.com", Message = "Login successful" };
            var token = "generated-jwt-token";

            _mockAuthService.Setup(s => s.LoginAsync(request.Email, request.Password))
                .ReturnsAsync(user);
            _mockTokenService.Setup(s => s.GenerateToken(user, "Admin"))
                .Returns(token);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponse>(okResult.Value);
            
            Assert.Equal(token, response.Token);
            Assert.Equal("Admin", response.Role); 
        }

        [Fact]
        public async Task Login_InvalidUser_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequest { Email = "wrong@test.com", Password = "wrong" };
            
            _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((User)null); // Service returns null

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task UpdatePassword_Success_ReturnsOk()
        {
            // Arrange
            var request = new UpdatePasswordRequest 
            { 
                DefaultPassword = "old", 
                NewPassword = "new", 
                ConfirmNewPassword = "new" 
            };
            
            var serviceResult = new PasswordUpdateResult { Success = true, Message = "Updated" };

            _mockAuthService.Setup(s => s.UpdatePasswordByDefaultAsync("old", "new", "new"))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdatePasswordByDefault(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UpdatePasswordResponse>(okResult.Value);
            Assert.True(response.Success);
        }
    }
}