using System.Threading.Tasks;
using Incident.Domain.Models;

namespace Incident.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> LoginAsync(string Email, string password);
    }
}