using System.Data;
using Dapper;
using Incident.Application.Filters;
using Incident.Application.Interfaces;
using Incident.Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq; 

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
            using var conn = new NpgsqlConnection(_connStr);
            var p = new DynamicParameters();

            p.Add("FilePath", filePath, DbType.String);
            p.Add("FileName", fileName, DbType.String);
            p.Add("FileSize", fileSize, DbType.Int64);

            _logger.LogInformation("Executing sp_UploadCSV for {FileName}", fileName);

            var uploadId = await conn.ExecuteScalarAsync<long>(
                sql: "SELECT \"sp_uploadcsv\"(@FilePath, @FileName, @FileSize)",
                param: p
            );

            return uploadId;
        }

        public async Task<IEnumerable<UploadHistory>> GetAsync(UploadHistoryFilter filter, CancellationToken ct = default)
        {
            using var conn = new NpgsqlConnection(_connStr);

            var p = new DynamicParameters();
            p.Add("SearchText", filter.SearchText);
            p.Add("SortBy", filter.SortBy);
            p.Add("SortDir", filter.SortDir);
            p.Add("PageNumber", filter.PageNumber);
            p.Add("PageSize", filter.PageSize);

            _logger.LogDebug("Calling sp_GetUploadHistory {@p}", new { filter.SearchText, filter.SortBy, filter.SortDir, filter.PageNumber, filter.PageSize });

            return await conn.QueryAsync<UploadHistory>(
                "SELECT * FROM \"sp_getuploadhistory\"(@SearchText, @SortBy, @SortDir, @PageNumber, @PageSize)",
                p
            );
        }

        public async Task<ImportSummary> ExecuteImportAsync(int uploadHistoryId, CancellationToken ct = default)
        {
            using var conn = new NpgsqlConnection(_connStr);
            await conn.OpenAsync(ct);

            var p = new DynamicParameters();

            p.Add("p_upload_id", uploadHistoryId, DbType.Int64);

            _logger.LogDebug("Calling sp_ImportIncidentsFromUpload {@UploadHistoryId}", uploadHistoryId);

            try
            {
                var row = await conn.QuerySingleAsync<ImportSummary>(
                    sql: "SELECT * FROM \"sp_importincidentsfromupload\"(@p_upload_id)",
                    param: p
                );

                return row;
            }
            catch (NpgsqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error while executing sp_ImportIncidentsFromUpload for id {Id}", uploadHistoryId);
                throw;
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogError(invEx, "sp_ImportIncidentsFromUpload returned no rows for id {Id}", uploadHistoryId);
                throw new InvalidOperationException($"Import stored procedure did not return summary for UploadHistoryId {uploadHistoryId}");
            }
        }
    }
}