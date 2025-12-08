using Incident.Application.Dtos.Requests;
using Incident.Application.Exceptions;
using Incident.Application.Interfaces;
using Incident.Application.Options;
using Incident.Application.Services;
using Incident.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Incident.Tests.Services
{
    public class FileIngestionServiceTests
    {
        private readonly Mock<IFileUploadRepository> _mockRepo;
        private readonly Mock<ILogger<FileIngestionService>> _mockLogger;
        private readonly FileIngestionService _service;
        private readonly StorageOptions _storageOptions;
        private readonly IngestionOptions _ingestionOptions;

        public FileIngestionServiceTests()
        {
            _mockRepo = new Mock<IFileUploadRepository>();
            _mockLogger = new Mock<ILogger<FileIngestionService>>();

            _storageOptions = new StorageOptions { MaxUploadBytes = 100, CsvRoot = "C:/Temp" };
            _ingestionOptions = new IngestionOptions();

            var storageMock = new Mock<IOptions<StorageOptions>>();
            storageMock.Setup(x => x.Value).Returns(_storageOptions);

            var ingestionMock = new Mock<IOptions<IngestionOptions>>();
            ingestionMock.Setup(x => x.Value).Returns(_ingestionOptions);

            _service = new FileIngestionService(
                _mockRepo.Object, 
                storageMock.Object, 
                ingestionMock.Object, 
                _mockLogger.Object);
        }

        [Fact]
        public async Task IngestAsync_NullRequest_ThrowsUnsupportedFormat()
        {
            await Assert.ThrowsAsync<UnsupportedFormatException>(
                () => _service.IngestAsync(null));
        }

        [Fact]
        public async Task IngestAsync_FileTooLarge_ThrowsFileTooLargeException()
        {
            // FIX: added Content = new MemoryStream()
            var request = new FileIngestionRequest 
            { 
                Length = 200, 
                FileName = "test.xlsx",
                Content = new MemoryStream() 
            }; 
            
            await Assert.ThrowsAsync<FileTooLargeException>(
                () => _service.IngestAsync(request));
        }

        [Fact]
        public async Task IngestAsync_InvalidExtension_ThrowsUnsupportedFormat()
        {
            // FIX: added Content = new MemoryStream()
            var request = new FileIngestionRequest 
            { 
                Length = 50, 
                FileName = "test.txt",
                Content = new MemoryStream() 
            };
            
            await Assert.ThrowsAsync<UnsupportedFormatException>(
                () => _service.IngestAsync(request));
        }

        [Fact]
        public async Task GetAsync_CallsRepository()
        {
            // Arrange
            var filter = new UploadHistoryFilter();
            var expectedData = new List<UploadHistory>();

            _mockRepo.Setup(r => r.GetAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            // Act
            await _service.GetAsync(filter);

            // Assert
            _mockRepo.Verify(r => r.GetAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ImportFromUploadAsync_ValidId_CallsRepository()
        {
            // Arrange
            int id = 1;
            var summary = new ImportSummary { InsertedCount = 5 };

            _mockRepo.Setup(r => r.ExecuteImportAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summary);

            // Act
            var result = await _service.ImportFromUploadAsync(id);

            // Assert
            Assert.Equal(5, result.InsertedCount);
        }

        [Fact]
        public async Task ImportFromUploadAsync_InvalidId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.ImportFromUploadAsync(0));
        }
    }
}