using Incident.Application.Dtos.Requests;
using Incident.Domain.Entities;


namespace Incident.Application.Interfaces
{
    public interface IFileIngestionService
    {
       Task<FileIngestionResult> IngestAsync(FileIngestionRequest request, CancellationToken ct = default);
       Task<IEnumerable<UploadHistory>> GetAsync(UploadHistoryFilter filter, CancellationToken ct = default);
       Task<ImportSummary> ImportFromUploadAsync(int uploadHistoryId, CancellationToken ct = default);
    }
}