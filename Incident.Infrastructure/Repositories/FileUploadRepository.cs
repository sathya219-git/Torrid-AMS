using System.Data;
using Dapper;
using Incident.Application.Filters;
using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Incident.Infrastructure.Repositories
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly string _connStr;
        private readonly ILogger<FileUploadRepository> _logger;

        public FileUploadRepository(IConfiguration cfg, ILogger<FileUploadRepository> logger)
        {
            _connStr = cfg.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("DefaultConnection missing");
            _logger = logger;
        }

        public async Task<long> SaveFileUploadAsync(string filePath, string fileName, long fileSize)
        {
            using var conn = new SqlConnection(_connStr);
            var p = new DynamicParameters();
            p.Add("@FilePath", filePath, DbType.String, size: 1024);
            p.Add("@FileName", fileName, DbType.String, size: 255);
            p.Add("@FileSize", fileSize, DbType.Int64);

            _logger.LogInformation("Executing dbo.UploadCSV for {FileName}", fileName);

            var uploadId = await conn.ExecuteScalarAsync<long>(
                sql: "dbo.sp_UploadCSV",
                param: p,
                commandType: CommandType.StoredProcedure);

            return uploadId;
        }
        public async Task<IEnumerable<UploadHistory>> GetAsync(UploadHistoryFilter filter, CancellationToken ct = default)
        {
            using var conn = new SqlConnection(_connStr);

            var p = new DynamicParameters();
            p.Add("@SearchText",  filter.SearchText, DbType.String);
            p.Add("@SortBy",      filter.SortBy,     DbType.String);
            p.Add("@SortDir",     filter.SortDir,    DbType.String);
            p.Add("@PageNumber",  filter.PageNumber, DbType.Int32);
            p.Add("@PageSize",    filter.PageSize,   DbType.Int32);

            _logger.LogDebug("EXEC dbo.sp_GetUploadHistory {@p}", new { filter.SearchText, filter.SortBy, filter.SortDir, filter.PageNumber, filter.PageSize });

            return await conn.QueryAsync<UploadHistory>(
                "dbo.sp_GetUploadHistory",
                p,
                commandType: CommandType.StoredProcedure);
        }
    }
}
