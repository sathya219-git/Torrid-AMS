using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Incident.API.Dtos.Requests
{
    public class FileUploadRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
