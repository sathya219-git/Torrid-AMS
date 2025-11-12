using Incident.Application.Filters;
using Incident.Domain.Models;

namespace Incident.Application.Interfaces
{
    public interface IFileUploadRepository
    {
        Task<long> SaveFileUploadAsync(string filePath, string fileName, long fileSize);
        Task<IEnumerable<UploadHistory>> GetAsync(UploadHistoryFilter filter, CancellationToken ct = default);
        Task<ImportSummary> ExecuteImportAsync(int uploadHistoryId, CancellationToken ct = default);
    }
}