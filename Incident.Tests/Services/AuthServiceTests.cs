using Incident.Application.Interfaces;
using Incident.Application.Models;
using Incident.Application.Services;
using Incident.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Incident.Tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ReturnsUser_OnSuccess()
        {
            var mockRepo = new Mock<IAuthRepository>();
            var mockLogger = new Mock<ILogger<AuthService>>();

            mockRepo.Setup(r => r.LoginAsync("john", "password"))
                    .ReturnsAsync(new User
                    {
                        UserID = 1,
                        Username = "john",
                        Email = "john@example.com",
                        Role = "Admin",
                        Message = "Login successful"
                    });

            var service = new AuthService(mockRepo.Object, mockLogger.Object);

            var result = await service.LoginAsync("john", "password");

            Assert.NotNull(result);
            Assert.Equal("john", result.Username);
            Assert.Equal("Admin", result.Role);
        }
    }
}
