using Incident.API.Dtos.Requests;
using Incident.API.DTOs.Requests;
using Incident.API.DTOs.Responses;
using Incident.Application.Exceptions;
using Incident.Application.Filters;
using Incident.Application.Interfaces;
using Incident.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Incident.Application.Helpers;
using Incident.API.Dtos.Responses;


namespace Incident.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileIngestionService _fileIngestionService;


        public FileUploadController(IFileIngestionService fileIngestionService)
        {
            _fileIngestionService = fileIngestionService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [RequestSizeLimit(20_000_000)]
        [ProducesResponseType(typeof(FileIngestionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upload([FromForm] FileUploadRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid || request.File is null || request.File.Length == 0)
                return BadRequest("No file provided.");

            try
            {
                using var stream = request.File.OpenReadStream();
                var appRequest = new FileIngestionRequest
                {
                    Content = stream,
                    FileName = request.File.FileName,
                    Length = request.File.Length
                };

                var result = await _fileIngestionService.IngestAsync(appRequest, ct);

                var response = new FileIngestionResponse
                {
                    UploadID = result.UploadID,
                    OriginalFileName = result.OriginalFileName,
                    FileSizeBytes = result.FileSizeBytes,
                    CsvPath = result.CsvPath,
                    Message = result.Message
                };

                return Ok(response);
            }
            catch (HeaderValidationException ex)
            {
                return BadRequest($"Header validation failed: {ex.Message}");
            }
            catch (FileTooLargeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnsupportedFormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while processing the file.");
            }
        }

        [HttpGet("history")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedUploadHistoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistory([FromQuery] UploadHistoryQueryRequest request, CancellationToken ct)
        {
            var filter = new UploadHistoryFilter
            {
                SearchText = request.SearchText,
                SortBy = request.SortBy,
                SortDir = request.SortDir,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var rows = (await _fileIngestionService.GetAsync(filter, ct)).ToList();
            var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;
            var totalPages = filter.PageSize > 0
                ? (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                : 0;

            var response = new PagedUploadHistoryResponse
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                Items = rows.Select(r => new UploadHistoryItemResponse
                {
                    ID = r.ID,
                    FileName = r.FileName,
                    FileSize = FilterHelper.FormatSize(r.FileSize).ToString(),
                    UploadedDate = r.Uploaded_At
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost("import")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ImportSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Import([FromQuery] int uploadHistoryId, CancellationToken ct)
        {
            if (uploadHistoryId <= 0)
                return BadRequest("Invalid UploadHistoryId.");

            try
            {
                var summary = await _fileIngestionService.ImportFromUploadAsync(uploadHistoryId, ct);

                var resp = new ImportSummaryResponse
                {
                    StagingRowCount = summary.StagingRowCount,
                    InsertedCount = summary.InsertedCount,
                    UpdatedCount = summary.UpdatedCount,
                    MatchedButNotUpdatedCount = summary.MatchedButNotUpdatedCount,
                    SkippedDueToMissingNumber = summary.SkippedDueToMissingNumber
                };

                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while importing incidents.");
            }
        }
    }
}