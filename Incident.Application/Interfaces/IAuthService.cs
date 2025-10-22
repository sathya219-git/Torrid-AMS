using Incident.Domain.Models;
using System.Threading.Tasks;

namespace Incident.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);
    }
}
