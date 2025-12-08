using System.Threading.Tasks;
using Incident.Domain.Entities;

namespace Incident.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> LoginAsync(string Email, string password);
        Task<PasswordUpdateResult> UpdatePasswordByDefaultAsync(string defaultPassword,string newPassword, string confirmNewPassword);
    
    }
}