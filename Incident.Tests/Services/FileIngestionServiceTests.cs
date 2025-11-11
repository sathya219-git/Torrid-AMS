using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Incident.Application.Exceptions;
using Incident.Application.Interfaces;
using Incident.Application.Models;
using Incident.Application.Options;
using Incident.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Incident.Tests.Services
{
  public class FileIngestionServiceTests
    {
        private static FileIngestionService CreateService(
            IFileUploadRepository? repoMock = null,
            long maxBytes = 10 * 1024 * 1024)
        {
            var repo = repoMock ?? new Mock<IFileUploadRepository>().Object;

            var storage = Options.Create(new StorageOptions
            {
                CsvRoot = Path.Combine(Path.GetTempPath(), "ims-tests"),
                MaxUploadBytes = maxBytes
            });

            var ingestion = Options.Create(new IngestionOptions
            {
                ExpectedHeaders = new()
                {
                    "Number","Opened","ShortDescription","Caller","Priority","State","Category",
                    "AssignmentGroup","AssignedTo","Updated","UpdatedBy","ChildIncidents","SlaDue",
                    "Severity","Subcategory","ResolutionNotes","Resolved","SlaCalculation",
                    "ParentIncident","Parent","TaskType"
                },
                RequireExactOrder = false
            });

            var logger = Mock.Of<ILogger<FileIngestionService>>();
            return new FileIngestionService(repo, storage, ingestion, logger);
        }

        [Fact]
        public async Task IngestAsync_EmptyFile_ThrowsUnsupportedFormat()
        {
            // Arrange
            var service = CreateService();
            var req = new FileIngestionRequest
            {
                Content = new MemoryStream(Array.Empty<byte>()),
                FileName = "incidents.xlsx",
                Length = 0
            };

            // Act
            var act = async () => await service.IngestAsync(req, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnsupportedFormatException>()
                     .WithMessage("*Empty file*");
        }

        [Fact]
        public async Task IngestAsync_TooLarge_ThrowsFileTooLarge()
        {
            // Arrange
            var service = CreateService(maxBytes: 5);
            var req = new FileIngestionRequest
            {
                Content = new MemoryStream(Encoding.UTF8.GetBytes("123456")),
                FileName = "incidents.xlsx",
                Length = 6
            };

            // Act
            var act = async () => await service.IngestAsync(req, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FileTooLargeException>();
        }

        [Fact]
        public async Task IngestAsync_UnsupportedExtension_ThrowsUnsupportedFormat()
        {
            // Arrange
            var service = CreateService();
            var req = new FileIngestionRequest
            {
                Content = new MemoryStream(Encoding.UTF8.GetBytes("content")),
                FileName = "incidents.txt",
                Length = 7
            };

            // Act
            var act = async () => await service.IngestAsync(req, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnsupportedFormatException>()
                     .WithMessage("*Only .xls/.xlsx*");
        }
    }
}
