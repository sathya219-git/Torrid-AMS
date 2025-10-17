using Dapper;
using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
 
namespace Incident.Infrastructure.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly string _connectionString;
 
        public IncidentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
 
        public async Task<IEnumerable<IncidentModel>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<IncidentModel>("GetAllIncidents", commandType: System.Data.CommandType.StoredProcedure);
        }

    }
}
 