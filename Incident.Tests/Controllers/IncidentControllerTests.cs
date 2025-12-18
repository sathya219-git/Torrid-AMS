using IncidentAPI.Controllers;
using Incident.Application.Dtos.Requests;
using Incident.Application.Dtos.Responses;
using Incident.Application.Interfaces;
using Incident.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Incident.Tests.Controllers
{
    public class IncidentControllerTests
    {
        private readonly Mock<IIncidentService> _mockIncidentService;
        private readonly IncidentController _controller;

        public IncidentControllerTests()
        {
            _mockIncidentService = new Mock<IIncidentService>();
            _controller = new IncidentController(_mockIncidentService.Object);
        }

        [Fact]
        public async Task GetDashboardKpis_ReturnsOk_WithCorrectData()
        {
            var request = new DashboardFilterRequest();
            var serviceResult = new DashboardKpi 
            { 
                TotalIncidents = 100, 
                Open_Count = 20,
                Breached_Count = 5,
                Open_More_15_Days = 12,
                Open_Less_15_Days = 8,
                StateCounts = new Dictionary<string, int> 
                { 
                    { "New", 5 }, 
                    { "In Progress", 15 } 
                }
            };
        
            _mockIncidentService.Setup(s => s.GetDashboardKpisAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            var result = await _controller.GetDashboardKpis(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DashboardKpiResponse>(okResult.Value);

            Assert.Equal(100, response.TotalIncidents);
            Assert.Equal(20, response.OpenCount);
            Assert.Equal(5, response.BreachedCount);
            Assert.Equal(12, response.OpenMore15Days);
            Assert.NotNull(response.States);
            Assert.Equal(2, response.States.Count);
            Assert.Equal(5, response.States["New"]);
            Assert.Equal(15, response.States["In Progress"]);

        }

        // 2. Name & Count (Paginated)
        [Fact]
        public async Task GetNameAndIncidentCount_ReturnsOk_WithPagination()
        {
            // Arrange
            var request = new DashboardFilterPaginatedRequest { PageNumber = 2, PageSize = 10 };
            var serviceResult = new PagedMemberIncidentStats
            {
                MemberDetails = new List<NameAndIncidentCountByPriority> 
                { 
                    new NameAndIncidentCountByPriority { Name = "John", TotalCount = 5 } 
                },
                Pagination = new PaginationInfo { Page = 2, TotalRecords = 50 }
            };

            _mockIncidentService.Setup(s => s.GetNameAndIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetNameAndIncidentCountByPriority(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MemberIncidentStatsResponse>(okResult.Value);
            
            Assert.Equal(2, response.Pagination.Page);
            Assert.Single(response.MemberDetails);
            Assert.Equal("John", response.MemberDetails.First().Name);
        }

        // 3. Assignment Groups
        [Fact]
        public async Task GetAssignmentGroups_ReturnsList()
        {
            // Arrange
            var request = new DashboardFilterRequest();
            var serviceResult = new List<AssignmentGroup>
            {
                new AssignmentGroup { AssignmentGroupName = "Support", IncidentCount = 10 }
            };

            _mockIncidentService.Setup(s => s.GetAssignmentGroupsAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetAssignmentGroups(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<AssignmentGroupResponse>>(okResult.Value);
            Assert.Single(response);
            Assert.Equal("Support", response[0].AssignmentGroupName);
        }

        // 4. Status Count
        [Fact]
        public async Task GetStatusCountByPriority_ReturnsList()
        {
            // Arrange
            var request = new DashboardFilterRequest();
            var serviceResult = new List<StatusCountByPriority>
            {
                new StatusCountByPriority { Status = "New", IncidentCount = 5 }
            };

            _mockIncidentService.Setup(s => s.GetStatusCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetStatusCountByPriority(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<StatusCountByPriorityResponse>>(okResult.Value);
            Assert.Single(response);
            Assert.Equal("New", response[0].Status);
        }

        // 5. Category Count
        [Fact]
        public async Task GetCategoryCountByGroup_ReturnsList()
        {
            // Arrange
            var request = new DashboardFilterRequest();
            var serviceResult = new List<CategoryCountByGroup>
            {
                new CategoryCountByGroup { CategoryName = "Hardware", IncidentCount = 8 }
            };

            _mockIncidentService.Setup(s => s.GetCategoryCountByGroupAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetCategoryCountByGroup(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<CategoryCountByGroupResponse>>(okResult.Value);
            Assert.Single(response);
            Assert.Equal("Hardware", response[0].CategoryName);
        }

        [Fact]
        public async Task GetIncidentCountByPriority_ReturnsGroupedResponse()
        {
            // Arrange
            var request = new DashboardFilterRequest();
            var serviceResult = new List<IncidentCountByPriority>
            {
                // Two items with same Priority "P1" but different States and Aging/Breach data
                new IncidentCountByPriority {
                    Priority = "P1",
                    State = "Open",
                    IncidentCount = 2,
                    TotalCount = 5,
                    BreachedCount = 1,
                    Open_more_15_days = 1,
                    Open_less_15_days = 1
                },
                new IncidentCountByPriority {
                    Priority = "P1",
                    State = "Closed",
                    IncidentCount = 3,
                    TotalCount = 5,
                    BreachedCount = 0,
                    Open_more_15_days = 0,
                    Open_less_15_days = 0
                }
            };

            _mockIncidentService.Setup(s => s.GetIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetIncidentCountByPriority(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<IncidentCountByPriorityGroupedResponse>(okResult.Value);

            // Verify P1 exists in the Dictionary
            Assert.True(response.Priority.ContainsKey("P1"));
            var p1Data = response.Priority["P1"];

            // 1. Verify Priority Level Totals
            Assert.Equal(5, p1Data.TotalCountForPriority);
            Assert.Equal(1, p1Data.BreachedCount); // Sum of both rows
            Assert.Equal(1, p1Data.OpenMoreThan15Days);

            // 2. Verify Dynamic State Mapping (The Automatic Handling)
            Assert.True(p1Data.StateDetails.ContainsKey("Open"));
            Assert.True(p1Data.StateDetails.ContainsKey("Closed"));

            Assert.Equal(2, p1Data.StateDetails["Open"]);
            Assert.Equal(3, p1Data.StateDetails["Closed"]);
        }

        // 7. Details By Priority (Paginated)
        [Fact]
        public async Task GetIncidentDetailsByPriority_ReturnsPaginatedList()
        {
            // Arrange
            var request = new DashboardFilterPaginatedRequest();
            var serviceResult = new List<IncidentDetailsByPriority>
            {
                // The service returns a list that implements the Paged properties via the first item usually
                new IncidentDetailsByPriority 
                { 
                    IncidentNo = "INC001", 
                    PageNumber = 1, 
                    TotalElements = 10 
                }
            };

            _mockIncidentService.Setup(s => s.GetIncidentDetailsByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetIncidentDetailsByPriority(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<IncidentDetailsPaginatedResponse>(okResult.Value);
            
            Assert.Equal(1, response.PageNumber);
            Assert.Single(response.Incidents);
            Assert.Equal("INC001", response.Incidents.First().IncidentNo);
        }

        // 8. Export Excel
        [Fact]
        public async Task ExportIncidents_ReturnsFileContent()
        {
            // Arrange
            var request = new DashboardFilterRequest();
            byte[] fakeFileBytes = new byte[] { 0xFF, 0x00, 0xFF }; // Fake Excel bytes

            _mockIncidentService.Setup(s => s.ExportIncidentsToExcelAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(fakeFileBytes);

            // Act
            var result = await _controller.ExportIncidents(request);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
            Assert.Equal(fakeFileBytes, fileResult.FileContents);
        }

        // 9. Breach List (Paginated)
        [Fact]
        public async Task GetBreachListByPriority_ReturnsPagedList()
        {
            // Arrange
            var request = new DashboardFilterPaginatedRequest();
            var serviceResult = new List<BreachListItem>
            {
                new BreachListItem { IncidentNumber = "INC00123", PageNumber = 1, TotalElements = 5 }
            };

            _mockIncidentService.Setup(s => s.GetBreachListByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetBreachListByPriority(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BreachListPagedResponse>(okResult.Value);
            
            Assert.Single(response.Items);
            Assert.Equal("INC00123", response.Items.First().IncidentNumber);
        }
    }
}