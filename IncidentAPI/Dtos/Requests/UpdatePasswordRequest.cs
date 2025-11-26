using System.ComponentModel.DataAnnotations;

namespace Incident.API.Dtos.Requests
{
    public class UpdatePasswordRequest
    {
        [Required]
        public string DefaultPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
