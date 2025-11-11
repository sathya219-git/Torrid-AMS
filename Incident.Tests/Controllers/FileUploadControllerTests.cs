using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Incident.API.Controllers;
using Incident.API.Dtos.Requests;
using Incident.Application.Exceptions;
using Incident.Application.Interfaces;
using Incident.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Incident.API.Dtos.Responses;
using Moq;
using Xunit;
using Incident.API.DTOs.Responses;

namespace Incident.Tests.API
{
    public class FileUploadControllerTests
    {
        private static IFormFile MakeFormFile(byte[] data, string fileName, string contentType)
        {
            var stream = new MemoryStream(data);
            return new FormFile(stream, 0, data.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Fact]
        public async Task Upload_NoFile_ReturnsBadRequest()
        {
            // Arrange
            var svc = new Mock<IFileIngestionService>();
            var controller = new FileUploadController(svc.Object);
            var req = new FileUploadRequest { File = null! };

            // Act
            var result = await controller.Upload(req, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value!.ToString().Should().Contain("No file provided");
        }

        [Fact]
        public async Task Upload_HeaderValidationFails_ReturnsBadRequest()
        {
            // Arrange
            var svc = new Mock<IFileIngestionService>();
            svc.Setup(s => s.IngestAsync(It.IsAny<FileIngestionRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new HeaderValidationException("Missing required headers"));

            var controller = new FileUploadController(svc.Object);

            var file = MakeFormFile(Encoding.UTF8.GetBytes("dummy"), "incidents.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            var req = new FileUploadRequest { File = file };

            // Act
            var result = await controller.Upload(req, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value!.ToString().Should().Contain("Header validation failed");
        }

        [Fact]
        public async Task Upload_ServiceSucceeds_ReturnsOkWithResponse()
        {
            // Arrange
            var svc = new Mock<IFileIngestionService>();
            svc.Setup(s => s.IngestAsync(It.IsAny<FileIngestionRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new FileIngestionResult
               {
                   UploadID = 99,
                   OriginalFileName = "incidents.xlsx",
                   FileSizeBytes = 1234,
                   CsvPath = @"D:\ims-uploads\csv\20250101\abc\incidents.csv",
                   Message = "ok"
               });

            var controller = new FileUploadController(svc.Object);

            var file = MakeFormFile(Encoding.UTF8.GetBytes("dummy"), "incidents.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            var req = new FileUploadRequest { File = file };

            // Act
            var result = await controller.Upload(req, CancellationToken.None);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;

            // Cast to the actual response DTO and assert properties
            ok.Value.Should().NotBeNull();
            ok.Value.Should().BeOfType<FileIngestionResponse>();
            var resp = (FileIngestionResponse)ok.Value!;


            resp.UploadID.Should().Be(99);
            resp.OriginalFileName.Should().Be("incidents.xlsx");
            resp.FileSizeBytes.Should().Be(1234);
            resp.CsvPath.Should().Contain(@"ims-uploads");
            resp.Message.Should().NotBeNullOrEmpty();
        }
    }
}
