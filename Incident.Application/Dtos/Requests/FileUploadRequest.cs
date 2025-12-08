using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Incident.Application.Dtos.Requests
{
    public class FileUploadRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
