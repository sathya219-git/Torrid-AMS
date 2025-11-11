using Incident.Application.Filters;
using Incident.Application.Models;
using Incident.Domain.Models;


namespace Incident.Application.Interfaces
{
    public interface IFileIngestionService
    {
       Task<FileIngestionResult> IngestAsync(FileIngestionRequest request, CancellationToken ct = default);
       Task<IEnumerable<UploadHistory>> GetAsync(UploadHistoryFilter filter, CancellationToken ct = default);
    }
}