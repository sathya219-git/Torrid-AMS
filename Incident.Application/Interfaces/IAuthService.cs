using Incident.Domain.Entities;
using System.Threading.Tasks;

namespace Incident.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string Email, string password);
        Task<PasswordUpdateResult> UpdatePasswordByDefaultAsync(string defaultPassword,string newPassword, string confirmNewPassword);
    
    }
}
