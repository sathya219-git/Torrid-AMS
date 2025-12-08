using Incident.API.Controllers;
using Incident.Application.Dtos.Requests;
using Incident.Application.Dtos.Responses;
using Incident.Application.Interfaces;
using Incident.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Incident.Tests.Controllers
{
    public class FileUploadControllerTests
    {
        private readonly Mock<IFileIngestionService> _mockFileService;
        private readonly FileUploadController _controller;

        public FileUploadControllerTests()
        {
            _mockFileService = new Mock<IFileIngestionService>();
            _controller = new FileUploadController(_mockFileService.Object);
        }

        [Fact]
        public async Task Upload_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "fake file content";
            var fileName = "test.csv";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var request = new FileUploadRequest { File = fileMock.Object };
            var serviceResult = new FileIngestionResult { UploadID = 123, Message = "Success" };

            _mockFileService.Setup(s => s.IngestAsync(It.IsAny<FileIngestionRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.Upload(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<FileIngestionResponse>(okResult.Value);
            Assert.Equal(123, response.UploadID);
        }

        [Fact]
        public async Task Upload_NullFile_ReturnsBadRequest()
        {
            // Arrange
            var request = new FileUploadRequest { File = null };

            // Act
            var result = await _controller.Upload(request, CancellationToken.None);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file provided.", badRequest.Value);
        }

        [Fact]
        public async Task GetHistory_ReturnsPagedResponse()
        {
            // Arrange
            var request = new UploadHistoryQueryRequest { PageNumber = 1, PageSize = 10 };
            var historyData = new List<UploadHistory>
            {
                new UploadHistory { ID = 1, FileName = "test.csv", TotalCount = 1 }
            };

            _mockFileService.Setup(s => s.GetAsync(It.IsAny<UploadHistoryFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(historyData);

            // Act
            var result = await _controller.GetHistory(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedUploadHistoryResponse>(okResult.Value);
            Assert.Single(response.Items);
        }
    }
}