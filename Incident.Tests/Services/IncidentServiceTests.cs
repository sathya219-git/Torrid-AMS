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
        public async Task GetNameAndIncidentCountByPriorityAsync_ReturnsPagedData()
        {
            // Arrange
            var filter = new IncidentFilter 
            { 
                PageNumber = 1, 
                PageSize = 4, 
                AssignedToName = new List<string> { "John" } // ✅ FIXED: list instead of string
            };

            var pagedData = new PagedMemberIncidentStats
            {
                MemberDetails = new List<NameAndIncidentCountByPriority>
                {
                    new NameAndIncidentCountByPriority
                    {
                        Name = "John",
                        P1 = 5,
                        P2 = 3,
                        P3 = 1,
                        P4 = 0,
                        ActualResolvedTime = "4 hours"
                    }
                },
                Pagination = new PaginationInfo
                {
                    Page = 1,
                    PageSize = 4,
                    TotalRecords = 1,
                    TotalPages = 1,
                    SortBy = "Alphabetical"
                }
            };

            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetNameAndIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                    .ReturnsAsync(pagedData);

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await service.GetNameAndIncidentCountByPriorityAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.MemberDetails);
            Assert.Equal("John", result.MemberDetails.First().Name);
            Assert.Equal(1, result.Pagination.TotalRecords);
        }


        [Fact]
        public async Task GetAssignmentGroupsAsync_ReturnsData()
        {
            var filter = new IncidentFilter();

            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetAssignmentGroupsAsync(filter))
                .ReturnsAsync(new List<AssignmentGroup>
                {
                    new AssignmentGroup { AssignmentGroupName = "Network Team" },
                    new AssignmentGroup { AssignmentGroupName = "DBA Team" }
                });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetAssignmentGroupsAsync(filter);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.AssignmentGroupName == "Network Team");
        }

        [Fact]
        public async Task GetIncidentCountByPriorityAsync_ReturnsData()
        {
        // Arrange
        var filter = new IncidentFilter { };

        var repoData = new List<IncidentCountByPriority>
        {
            new IncidentCountByPriority
            {
                Priority = "P1 - Critical",
                State = "Open",
                IncidentCount = 10,
                TotalCount = 30,
                AvgResolvedTime = "2 hours",
                TotalResolvedTime = "150 hours"
            },
            new IncidentCountByPriority
            {
                Priority = "P1 - Critical",
                State = "Closed",
                IncidentCount = 20,
                TotalCount = 30,
                AvgResolvedTime = "2 hours",
                TotalResolvedTime = "150 hours"
            }
        };

        var mockRepo = new Mock<IIncidentRepository>();
        mockRepo.Setup(r => r.GetIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(repoData);

        var mockLogger = new Mock<ILogger<IncidentService>>();
        var service = new IncidentService(mockRepo.Object, mockLogger.Object);

        // Act
        var result = await service.GetIncidentCountByPriorityAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("P1 - Critical", result.First().Priority);
        Assert.Equal(30, result.First().TotalCount);
        Assert.Equal("2 hours", result.First().AvgResolvedTime);
        }

            
        [Fact]
        public async Task GetCategoryCountByGroupAsync_ReturnsData()
        {
            var filter = new IncidentFilter();

            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetCategoryCountByGroupAsync(filter))
                .ReturnsAsync(new List<CategoryCountByGroup>
                {
                    new CategoryCountByGroup { CategoryName = "Hardware", IncidentCount = 10 },
                    new CategoryCountByGroup { CategoryName = "Software", IncidentCount = 5 }
                });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetCategoryCountByGroupAsync(filter);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.CategoryName == "Hardware");
        }

        [Fact]
        public async Task GetStatusCountByPriorityAsync_ReturnsData()
        {
            var filter = new IncidentFilter();

            var mockRepo = new Mock<IIncidentRepository>();
            mockRepo.Setup(r => r.GetStatusCountByPriorityAsync(filter))
                .ReturnsAsync(new List<StatusCountByPriority>
                {
                    new StatusCountByPriority { Status = "Open", IncidentCount = 10 },
                    new StatusCountByPriority { Status = "Closed", IncidentCount = 5 }
                });

            var mockLogger = new Mock<ILogger<IncidentService>>();
            var service = new IncidentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetStatusCountByPriorityAsync(filter);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Status == "Open");
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
        
        [Fact]
    public async Task GetIncidentDetailsByPriorityAsync_ReturnsFilteredResults()
    {
        // Arrange
        var filter = new IncidentFilter
        {
            PageNumber = 1,
            PageSize = 8,
            Search = "INC2233985"
        };

        var mockData = new List<IncidentDetailsByPriority>
        {
            new IncidentDetailsByPriority
            {
                PageNumber = 1,
                PageSize = 8,
                TotalPages = 3,
                TotalElements = 10,
                IncidentNo = "INC2233985",
                AssignedTo = "John Doe",
                ShortDescription = "Database outage",
                Category = "Infra",
                State = "Closed",
                ActualResolvedTime = "2 days 5 hours",
                ResolvedDateTime = DateTime.UtcNow,
                BreachSLA = "No Breach"
            }
        };

        var repo = new Mock<IIncidentRepository>();
        repo.Setup(r => r.GetIncidentDetailsByPriorityAsync(It.IsAny<IncidentFilter>()))
            .ReturnsAsync(mockData);

        var logger = new Mock<ILogger<IncidentService>>();
        var service = new IncidentService(repo.Object, logger.Object);

        // Act
        var result = await service.GetIncidentDetailsByPriorityAsync(filter);

        // Assert
        Assert.NotNull(result);
        var first = Assert.Single(result);
        Assert.Equal("INC2233985", first.IncidentNo);
        Assert.Equal("Infra", first.Category);
        Assert.Equal("Closed", first.State);
        Assert.Equal("No Breach", first.BreachSLA);
        Assert.Equal(10, first.TotalElements);
        Assert.Equal(1, first.PageNumber);
        Assert.Equal(8, first.PageSize);
    }
    }
}
