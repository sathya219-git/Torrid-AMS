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
        public async Task GetNameAndIncidentCountByPriority_ReturnsOk()
        {
            // Arrange
            var request = new DashboardFilterRequest(); // same request DTO
            var mockService = new Mock<IIncidentService>();

            mockService.Setup(s => s.GetNameAndIncidentCountByPriorityAsync(It.IsAny<IncidentFilter>()))
                       .ReturnsAsync(new List<NameAndIncidentCountByPriority>
                       {
                           new NameAndIncidentCountByPriority
                           {
                               AssignedToName = "John",
                               Priority = "High",
                               IncidentCount = 5
                           }
                       });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetNameAndIncidentCountByPriority(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<NameAndIncidentCountByPriorityResponse>>(okResult.Value);

            Assert.Single(response);
            Assert.Equal("John", response.First().AssignedToName);
            Assert.Equal("High", response.First().Priority);
            Assert.Equal(5, response.First().IncidentCount);
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
        public async Task GetIncidentDetailsByPriority_ReturnsOk()
        {
            var request = new DashboardFilterRequest();
            var mockService = new Mock<IIncidentService>();

            mockService.Setup(s => s.GetIncidentDetailsByPriorityAsync(It.IsAny<IncidentFilter>()))
                .ReturnsAsync(new List<IncidentDetails>
                {
                    new IncidentDetails
                    {
                        IncidentNumber = "INC1001",
                        CallerName = "Alice",
                        PriorityLevel = "High",
                        CategoryName = "Software",
                        AssignmentGroup = "Support Team",
                        AssignedTo = "John",
                        CurrentState = "In Progress"
                    }
                });

            var controller = new IncidentController(mockService.Object);

            var result = await controller.GetIncidentDetailsByPriority(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<IncidentDetailsResponse>>(okResult.Value);

            Assert.Single(response);
            Assert.Contains("INC1001", response.First().IncidentNumber);
            Assert.Contains("High", response.First().PriorityLevel);
            Assert.Contains("Software", response.First().CategoryName);
            Assert.Contains("John", response.First().AssignedTo);

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
    }
}
