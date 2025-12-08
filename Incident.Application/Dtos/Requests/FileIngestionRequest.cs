namespace Incident.Application.Dtos.Requests
{
    public class FileIngestionRequest
    {
        public required Stream Content { get; init; }      // file stream
        public required string FileName { get; init; }     // original name incl. extension
        public required long Length { get; init; }         // size in bytes
    }
}
