using IncidentAPI.Controllers;
using Incident.API.Dtos.Requests;
using Incident.API.Dtos.Responses;
using Incident.Application.Models;
using Incident.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Incident.Application.Interfaces;

namespace Incident.Tests.Controllers
{
    public class IncidentControllerTests
    {
        [Fact]
        public async Task GetDashboardKpis_ReturnsOk()
        {
            var request = new DashboardFilterRequest { FromDate = null, ToDate = null };

            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetDashboardKpisAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(new DashboardKpi
                       {
                           TotalIncidents = 5
                       });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetDashboardKpis(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DashboardKpiResponse>(okResult.Value);
            Assert.Equal(5, response.TotalIncidents);
        }

        [Fact]
        public async Task GetNameAndIncidentCountByPriority_ReturnsOkWithPaginatedResponse()
        {
            // Arrange
            var request = new DashboardFilterPaginatedRequest
            {
                PageNumber = 1,
                PageSize = 4,
                AssignedToName = new List<string> { "John" } // ✅ FIXED: list instead of string
            };

            var pagedResult = new PagedMemberIncidentStats
            {
                MemberDetails = new List<NameAndIncidentCountByPriority>
        {
            new NameAndIncidentCountByPriority
            {
                Name = "John",
                P1 = 5,
                P2 = 2,
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

            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetNameAndIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(pagedResult);

            var controller = new IncidentController(mockService.Object);

            // Act
            var result = await controller.GetNameAndIncidentCountByPriority(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MemberIncidentStatsResponse>(okResult.Value);

            Assert.NotNull(response);
            Assert.Single(response.MemberDetails);
            Assert.Equal("John", response.MemberDetails.First().Name);
            Assert.Equal(1, response.Pagination.TotalRecords);
        }

        [Fact]
        public async Task GetAssignmentGroups_ReturnsOk()
        {
            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetAssignmentGroupsAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(new List<AssignmentGroup>
                {
                    new AssignmentGroup { AssignmentGroupName = "Network Team" },
                    new AssignmentGroup { AssignmentGroupName = "DBA Team" }
                });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetAssignmentGroups(new DashboardFilterRequest());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<AssignmentGroupResponse>>(okResult.Value);

            Assert.Equal(2, response.Count());
            Assert.Contains(response, r => r.AssignmentGroupName == "Network Team");
        }

        [Fact]
        public async Task GetIncidentCountByPriority_ReturnsOk_WithNestedPriorityData()
        {
            // Arrange
            var request = new DashboardFilterRequest
            {
                Priority = new List<string> { "P1 - Critical" }
            };

            var fakeData = new List<IncidentCountByPriority>
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

            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                    .ReturnsAsync(fakeData);

            var controller = new IncidentController(mockService.Object);

            // Act
            var result = await controller.GetIncidentCountByPriority(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<IncidentCountByPriorityGroupedResponse>(ok.Value);

            Assert.NotNull(response.Priority);
            Assert.True(response.Priority.ContainsKey("P1 - Critical"));

            var priorityBlock = response.Priority["P1 - Critical"];
            Assert.NotNull(priorityBlock);
            Assert.Equal("2 hours", priorityBlock.AvgResolvedTime);
            Assert.Equal("150 hours", priorityBlock.TotalResolvedTime);

            var stateCounts = Assert.Single(priorityBlock.Details);
            Assert.Equal(30, stateCounts.TotalCount);
            Assert.Equal(10, stateCounts.Open);
            Assert.Equal(20, stateCounts.Closed);
            Assert.Equal(0, stateCounts.InProgress);
            Assert.Equal(0, stateCounts.OnHold);
            Assert.Equal(0, stateCounts.Reopen);
            Assert.Equal(0, stateCounts.Resolved);
        }

    [Fact]
    public async Task GetIncidentDetailsByPriority_ReturnsPagedResponse()
    {
        // Arrange
        var request = new DashboardFilterPaginatedRequest
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

        var service = new Mock<IIncidentService>();
        service.Setup(s => s.GetIncidentDetailsByPriorityAsync(It.IsAny<IncidentFilter>()))
               .ReturnsAsync(mockData);

        var controller = new IncidentController(service.Object);

        // Act
        var result = await controller.GetIncidentDetailsByPriority(request);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<IncidentDetailsPaginatedResponse>(ok.Value);

        Assert.Equal(1, response.PageNumber);
        Assert.Equal(8, response.PageSize);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(10, response.TotalElements);

        var item = Assert.Single(response.Incidents);
        Assert.Equal("INC2233985", item.IncidentNo);
        Assert.Equal("Infra", item.Category);
        Assert.Equal("Closed", item.State);
        Assert.Equal("No Breach", item.BreachSLA);
    }


        [Fact]
        public async Task GetCategoryCountByGroup_ReturnsOk()
        {
            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetCategoryCountByGroupAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(new List<CategoryCountByGroup>
                {
                    new CategoryCountByGroup { CategoryName = "Hardware", IncidentCount = 10 },
                    new CategoryCountByGroup { CategoryName = "Software", IncidentCount = 5 }
                });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetCategoryCountByGroup(new DashboardFilterRequest());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<CategoryCountByGroupResponse>>(okResult.Value);

            Assert.Equal(2, response.Count());
            Assert.Contains(response, r => r.CategoryName == "Hardware");
        }

        [Fact]
        public async Task GetStatusCountByPriority_ReturnsOk()
        {
            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetStatusCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(new List<StatusCountByPriority>
                {
                    new StatusCountByPriority { Status = "Open", IncidentCount = 10 },
                    new StatusCountByPriority { Status = "Closed", IncidentCount = 5 }
                });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetStatusCountByPriority(new DashboardFilterRequest());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<StatusCountByPriorityResponse>>(okResult.Value);

            Assert.Equal(2, response.Count());
            Assert.Contains(response, r => r.Status == "Open");
        }

        [Fact]
        public async Task GetStatusCountByPriority_ReturnsNotFound_WhenEmpty()
        {
            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetStatusCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(new List<StatusCountByPriority>());

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetStatusCountByPriority(new DashboardFilterRequest());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task ExportIncidents_ReturnsExcelFile_WithCorrectDTOData()
        {
            var mockService = new Mock<IIncidentService>();
            var request = new DashboardFilterRequest();

            var mockData = new List<ExportIncident>
            {
                new ExportIncident { Number = "INC001", Priority = "High", State = "Open" }
            };

            mockService.Setup(s => s.ExportIncidentsAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(mockData);

            var controller = new IncidentController(mockService.Object);

            var result = await controller.ExportIncidents(request);

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
            Assert.EndsWith(".xlsx", fileResult.FileDownloadName);
            Assert.NotNull(fileResult.FileContents);
            Assert.True(fileResult.FileContents.Length > 0);

            var dto = mockData.Select(i => new ExportIncidentResponse
            {
                Number = i.Number,
                Priority = i.Priority,
                State = i.State
            }).First();

            Assert.Equal("INC001", dto.Number);
            Assert.Equal("High", dto.Priority);
            Assert.Equal("Open", dto.State);
        }
    }
}
