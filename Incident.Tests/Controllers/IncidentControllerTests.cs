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
        public async Task GetNameAndIncidentCountByPriority_ReturnsPaginatedResponse()
        {
            var request = new DashboardFilterRequest { PageNumber = 1, PageSize = 4, Search = "John" };

            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetNameAndIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(new List<NameAndIncidentCountByPriority>
                       {
                   new NameAndIncidentCountByPriority { AssignedToName = "John", Priority = "High", IncidentCount = 10, AvgResolutionTime_Hours = 5.2, TotalCount = 40 }
                       });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetNameAndIncidentCountByPriority(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaginatedResponse<NameAndIncidentCountByPriorityResponse>>(okResult.Value);

            Assert.Single(response.Data);
            Assert.Equal(40, response.TotalCount);
            Assert.Equal("John", response.Data.First().AssignedToName);
        }

        [Fact]
        public async Task GetAssignmentGroups_ReturnsOk()
        {
            var request = new DashboardFilterRequest();
            var mockService = new Mock<IIncidentService>();

            mockService.Setup(s => s.GetAssignmentGroupsAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(new List<AssignmentGroup>
                       {
                   new AssignmentGroup { AssignmentGroupName = "Web Support" }
                       });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetAssignmentGroups(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<AssignmentGroupResponse>>(okResult.Value);

            Assert.Single(response);
            Assert.Equal("Web Support", response.First().AssignmentGroupName);
        }

        [Fact]
        public async Task GetIncidentCountByPriority_ReturnsOk()
        {
            var request = new DashboardFilterRequest();
            var mockService = new Mock<IIncidentService>();

            mockService.Setup(s => s.GetIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                 .ReturnsAsync(new List<IncidentCountByPriority>
                 {
                    new IncidentCountByPriority
                    {
                        Priority = "High",
                        IncidentCount = 12
                    }
                 });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetIncidentCountByPriority(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<IncidentCountByPriorityResponse>>(okResult.Value);

            Assert.Single(response);
            Assert.Equal("High", response.First().Priority);
            Assert.Equal(12, response.First().IncidentCount);
        }

        [Fact]
        public async Task GetIncidentDetailsByPriority_ReturnsPaginatedResponse()
        {
            var request = new DashboardFilterRequest { PageNumber = 1, PageSize = 8, Search = "INC001" };

            var mockService = new Mock<IIncidentService>();
            mockService.Setup(s => s.GetIncidentDetailsByPriorityAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(new List<IncidentDetailsByPriority>
                       {
                   new IncidentDetailsByPriority { IncidentNumber = "INC001", Description = "Test Incident", TotalCount = 50 }
                       });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetIncidentDetailsByPriority(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaginatedResponse<IncidentDetailsByPriorityResponse>>(okResult.Value);

            Assert.Single(response.Data);
            Assert.Equal(50, response.TotalCount);
            Assert.Equal("INC001", response.Data.First().IncidentNumber);
        }

        [Fact]
        public async Task GetCategoryCountByGroup_ReturnsOk()
        {
            var request = new DashboardFilterRequest();
            var mockService = new Mock<IIncidentService>();

            mockService.Setup(s => s.GetCategoryCountByGroupAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(new List<CategoryCountByGroup>
                       {
                   new CategoryCountByGroup { CategoryName = "Software", IncidentCount = 10 }
                       });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetCategoryCountByGroup(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<CategoryCountByGroupResponse>>(okResult.Value);

            Assert.Single(response);
            Assert.Equal("Software", response.First().CategoryName);
            Assert.Equal(10, response.First().IncidentCount);
        }

        [Fact]
        public async Task GetStatusCountByPriority_ReturnsOk()
        {
            var request = new DashboardFilterRequest();
            var mockService = new Mock<IIncidentService>();

            mockService.Setup(s => s.GetStatusCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(new List<StatusCountByPriority>
                       {
                   new StatusCountByPriority { Priority = "High", Status = "Open", IncidentCount = 3 }
                       });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetStatusCountByPriority(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<StatusCountByPriorityResponse>>(okResult.Value);

            Assert.Single(response);
            Assert.Equal("High", response.First().Priority);
            Assert.Equal("Open", response.First().Status);
            Assert.Equal(3, response.First().IncidentCount);
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
