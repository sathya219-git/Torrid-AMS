using Incident.Application.Dtos.Requests;
using Incident.Application.Interfaces;
using Incident.Application.Services;
using Incident.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Incident.Tests.Services
{
    public class IncidentServiceTests
    {
        private readonly Mock<IIncidentRepository> _mockRepo;
        private readonly Mock<ILogger<IncidentService>> _mockLogger;
        private readonly IncidentService _service;

        public IncidentServiceTests()
        {
            _mockRepo = new Mock<IIncidentRepository>();
            _mockLogger = new Mock<ILogger<IncidentService>>();
            _service = new IncidentService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetDashboardKpis_CallsRepository()
        {
            // Arrange
            var filter = new IncidentFilter();
            var expectedKpi = new DashboardKpi { TotalIncidents = 10 };

            _mockRepo.Setup(r => r.GetDashboardKpisAsync(filter))
                .ReturnsAsync(expectedKpi);

            // Act
            var result = await _service.GetDashboardKpisAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.TotalIncidents);
        }

        [Fact]
        public async Task ExportIncidentsToExcelAsync_GeneratesBytes()
        {
            // Arrange
            var filter = new IncidentFilter();
            var data = new List<ExportIncident>
            {
                new ExportIncident { Number = "INC001", Short_Description = "Test Issue" }
            };

            _mockRepo.Setup(r => r.ExportIncidentsAsync(filter))
                .ReturnsAsync(data);

            // Act
            var result = await _service.ExportIncidentsToExcelAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0); // Confirms ClosedXML generated a file
            _mockRepo.Verify(r => r.ExportIncidentsAsync(filter), Times.Once);
        }
    }
}