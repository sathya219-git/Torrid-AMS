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
    public class IncidentServiceTests
    {
        [Fact]
        public async Task GetDashboardKpisAsync_ReturnsData()
        {
            var filter = new IncidentFilter { FromDate = null, ToDate = null };

            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetDashboardKpisAsync(filter))
                    .ReturnsAsync(new DashboardKpi
                    {
                        TotalIncidents = 10,
                        NewIncidents = 2,
                        OpenIncidents = 3,
                        InProgressIncidents = 1,
                        OnHoldIncidents = 1,
                        ResolvedIncidents = 2,
                        ClosedIncidents = 1
                    });

            var mockLogger = new Mock<ILogger<IncidentService>>();

            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetDashboardKpisAsync(filter);

            Assert.NotNull(result);
            Assert.Equal(10, result.TotalIncidents);
        }


        [Fact]
        public async Task GetNameAndIncidentCountByPriorityAsync_ReturnsData()
        {
            var filter = new IncidentFilter { PageNumber = 1, PageSize = 4, Search = "John" };

            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetNameAndIncidentCountByPriorityAsync(filter))
                    .ReturnsAsync(new List<NameAndIncidentCountByPriority>
                    {
                new NameAndIncidentCountByPriority { AssignedToName = "John", Priority = "High", IncidentCount = 10, AvgResolutionTime_Hours = 5.2, TotalCount = 40 }
                    });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetNameAndIncidentCountByPriorityAsync(filter);

            Assert.Single(result);
            Assert.Equal("John", result.First().AssignedToName);
        }

        [Fact]
        public async Task GetAssignmentGroupsAsync_ReturnsData()
        {
            var filter = new IncidentFilter();
            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetAssignmentGroupsAsync(filter))
                    .ReturnsAsync(new List<AssignmentGroup>
                    {
                new AssignmentGroup { AssignmentGroupName = "IT Support" },
                new AssignmentGroup { AssignmentGroupName = "Network Team" }
                    });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetAssignmentGroupsAsync(filter);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.AssignmentGroupName == "IT Support");
        }

        [Fact]
        public async Task GetIncidentCountByPriorityAsync_ReturnsData()
        {
            var filter = new IncidentFilter();
            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetIncidentCountByPriorityAsync(filter))
                    .ReturnsAsync(new List<IncidentCountByPriority>
                    {
                        new IncidentCountByPriority
                        {
                            Priority = "High",
                            IncidentCount = 12
                        },
                        new IncidentCountByPriority
                        {
                            Priority = "Low",
                            IncidentCount = 05
                        }
                    });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetIncidentCountByPriorityAsync(filter);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Priority == "High" && r.IncidentCount == 12);
        }


        [Fact]
        public async Task GetIncidentDetailsByPriorityAsync_ReturnsData()
        {
            var filter = new IncidentFilter { PageNumber = 1, PageSize = 8, Search = "INC001" };

            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetIncidentDetailsByPriorityAsync(filter))
                    .ReturnsAsync(new List<IncidentDetailsByPriority>
                    {
                new IncidentDetailsByPriority { IncidentNumber = "INC001", Description = "Test Incident", TotalCount = 50 }
                    });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetIncidentDetailsByPriorityAsync(filter);

            Assert.Single(result);
            Assert.Equal("INC001", result.First().IncidentNumber);
        }
        
        [Fact]
        public async Task GetCategoryCountByGroupAsync_ReturnsData()
        {
            var filter = new IncidentFilter();
            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetCategoryCountByGroupAsync(filter))
                    .ReturnsAsync(new List<CategoryCountByGroup>
                    {
                new CategoryCountByGroup { CategoryName = "Software", IncidentCount = 10 },
                new CategoryCountByGroup { CategoryName = "Hardware", IncidentCount = 5 }
                    });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetCategoryCountByGroupAsync(filter);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.CategoryName == "Software" && r.IncidentCount == 10);
        }

        [Fact]
        public async Task GetStatusCountByPriorityAsync_ReturnsData()
        {
            var filter = new IncidentFilter();
            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetStatusCountByPriorityAsync(filter))
                    .ReturnsAsync(new List<StatusCountByPriority>
                    {
                new StatusCountByPriority { Priority = "High", Status = "Open", IncidentCount = 3 },
                new StatusCountByPriority { Priority = "Low", Status = "Closed", IncidentCount = 2 }
                    });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetStatusCountByPriorityAsync(filter);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Priority == "High" && r.Status == "Open");
        }

        [Fact]
        public async Task ExportIncidentsAsync_ReturnsData()
        {
            var filter = new IncidentFilter();
            var mockRepo = new Mock<IIncidentRepository>();
            var expected = new List<ExportIncident>
            {
                new ExportIncident { Number = "INC001", Priority = "High", State = "Open" }
            };

            mockRepo.Setup(r => r.ExportIncidentsAsync(It.IsAny<IncidentFilter>()))
                     .ReturnsAsync(expected);

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);
            var result = await service.ExportIncidentsAsync(new IncidentFilter());

            Assert.Single(result);
            Assert.Contains(result, r => r.Number == "INC001" && r.Priority == "High" && r.State == "Open");
        }

    }
}
