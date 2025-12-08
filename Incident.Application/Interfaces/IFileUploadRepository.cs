using Incident.Application.Dtos.Requests;
using Incident.Domain.Entities;

namespace Incident.Application.Interfaces
{
    public interface IFileUploadRepository
    {
        Task<long> SaveFileUploadAsync(string filePath, string fileName, long fileSize);
        Task<IEnumerable<UploadHistory>> GetAsync(UploadHistoryFilter filter, CancellationToken ct = default);
        Task<ImportSummary> ExecuteImportAsync(int uploadHistoryId, CancellationToken ct = default);
    }
}