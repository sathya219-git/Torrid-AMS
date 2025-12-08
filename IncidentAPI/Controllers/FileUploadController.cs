using Incident.Application.Dtos.Requests;
using Incident.Application.Dtos.Responses;
using Incident.Application.Exceptions;
using Incident.Application.Interfaces;
using IncidentAPI.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incident.API.Controllers
{
    [Authorize(Roles = "Admin")]
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

                using var stream = request.File.OpenReadStream();

                var appRequest = new FileIngestionRequest
                {
                    Content = stream,
                    FileName = request.File.FileName,
                    Length = request.File.Length
                };

                var result = await _fileIngestionService.IngestAsync(appRequest, ct);
                return Ok(result.ToResponse());
        }

        [HttpGet("history")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedUploadHistoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistory([FromQuery] UploadHistoryQueryRequest request, CancellationToken ct)
        {
            var filter = request.ToDomainFilter();

            var rows = (await _fileIngestionService.GetAsync(filter, ct)).ToList();
            return Ok(rows.ToPagedResponse(filter));
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

                var summary = await _fileIngestionService.ImportFromUploadAsync(uploadHistoryId, ct);
                return Ok(summary.ToResponse());
        }
    }
}