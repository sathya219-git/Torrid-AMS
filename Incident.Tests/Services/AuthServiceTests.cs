using Incident.Application.Interfaces;
using Incident.Application.Services;
using Incident.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Incident.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockRepo;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockRepo = new Mock<IAuthRepository>();
            _mockLogger = new Mock<ILogger<AuthService>>();
            _service = new AuthService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            string email = "test@test.com";
            string password = "password";
            var expectedUser = new User { Email = email, Username = "admin" };

            _mockRepo.Setup(r => r.LoginAsync(email, password))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.LoginAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Email, result.Email);
            _mockRepo.Verify(r => r.LoginAsync(email, password), Times.Once);
        }

        [Fact]
        public async Task UpdatePassword_CallsRepository()
        {
            // Arrange
            var resultObj = new PasswordUpdateResult { Success = true };
            _mockRepo.Setup(r => r.UpdatePasswordByDefaultAsync("old", "new", "new"))
                .ReturnsAsync(resultObj);

            // Act
            var result = await _service.UpdatePasswordByDefaultAsync("old", "new", "new");

            // Assert
            Assert.True(result.Success);
            _mockRepo.Verify(r => r.UpdatePasswordByDefaultAsync("old", "new", "new"), Times.Once);
        }
    }
}