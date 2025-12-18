using System.Text;
using ExcelDataReader;
using Incident.Application.Dtos.Requests;
using Incident.Application.Exceptions;
using Incident.Application.Interfaces;
using Incident.Application.Options;
using Incident.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Incident.Application.Services
{
    public class FileIngestionService : IFileIngestionService
    {
        private readonly IFileUploadRepository _repo;
        private readonly ILogger<FileIngestionService> _logger;
        private readonly StorageOptions _storage;
        private readonly IngestionOptions _ingestion;

        public FileIngestionService(
            IFileUploadRepository repo,
            IOptions<StorageOptions> storage,
            IOptions<IngestionOptions> ingestion,
            ILogger<FileIngestionService> logger)
        {
            _repo = repo;
            _storage = storage.Value;
            _ingestion = ingestion.Value;
            _logger = logger;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // needed for .xls
        }

        public async Task<FileIngestionResult> IngestAsync(FileIngestionRequest request, CancellationToken ct = default)
        {
            if (request is null || request.Length <= 0)
                throw new UnsupportedFormatException("Empty file.");

            if (request.Length > _storage.MaxUploadBytes)
                throw new FileTooLargeException($"File exceeds {_storage.MaxUploadBytes} bytes.");

            var originalName = SanitizeFileName(request.FileName);
            var ext = Path.GetExtension(originalName).ToLowerInvariant();
            if (ext != ".xls" && ext != ".xlsx")
                throw new UnsupportedFormatException("Only .xls/.xlsx supported.");

            var ymd = DateTime.UtcNow.ToString("yyyyMMdd");
            var outDir = Path.Combine(_storage.CsvRoot, ymd);
            Directory.CreateDirectory(outDir);
            var csvPath = Path.Combine(outDir, Path.ChangeExtension(originalName, ".csv"));

            using var reader = CreateExcelReader(request.Content, ext);
            var ds = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = false }
            });

            var table = ds.Tables[0];
            int headerRowIndex = 0; // Or use your loop to find the non-empty row
            var headerRow = table.Rows[headerRowIndex];

            // --- NEW LOOSE MATCH MAPPING LOGIC START ---

            var expectedHeaders = _ingestion.ExpectedHeaders ?? new List<string>();

            // 1. Build a Normalized Map of what is actually in the Excel file
            // Key: "sladue", Value: 17 (the index in the Excel row)
            var actualExcelMap = headerRow.ItemArray
                .Select((val, idx) => new {
                    NormalizedName = Normalize(val?.ToString()),
                    Index = idx
                })
                .Where(x => !string.IsNullOrEmpty(x.NormalizedName))
                .GroupBy(x => x.NormalizedName)
                .ToDictionary(g => g.Key, g => g.First().Index);

            // 2. Map Expected Headers to the Actual Indices
            // This creates an ordered list of "where to pluck from"
            var targetIndices = expectedHeaders
                .Select(h => {
                    var normExpected = Normalize(h);
                    if (!actualExcelMap.TryGetValue(normExpected, out int excelIdx))
                    {
                        throw new HeaderValidationException($"Required column '{h}' not found.");
                    }
                    return excelIdx;
                })
                .ToList();

            // 3. Write the CSV
            using (var fs = new FileStream(csvPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                // Write standard headers from appsettings (Order is guaranteed 1 to 21)
                await sw.WriteLineAsync(string.Join(",", expectedHeaders.Select(ToCsvField)));

                for (int r = headerRowIndex + 1; r < table.Rows.Count; r++)
                {
                    var excelRow = table.Rows[r];

                    // Pluck only the columns we want in the exact order we want them
                    var csvValues = targetIndices.Select((excelIdx, targetPos) =>
                    {
                        var cellValue = excelRow[excelIdx];
                        var headerName = expectedHeaders[targetPos];
                        return ConvertToCsvValue(cellValue, headerName);
                    });

                    await sw.WriteLineAsync(string.Join(",", csvValues));
                }
            }

            // Save metadata
            var uploadId = await _repo.SaveFileUploadAsync(csvPath, originalName, request.Length);
            _logger.LogInformation("Ingested file {File} -> {Csv} (UploadID {Id})", originalName, csvPath, uploadId);

            return new FileIngestionResult
            {
                UploadID = uploadId,
                OriginalFileName = originalName,
                FileSizeBytes = request.Length,
                CsvPath = csvPath,
                Message = "File uploaded, validated, converted, and recorded."
            };
        }

        private string ConvertToCsvValue(object cell, string header)
        {
            var unassignedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Priority", "State", "Category", 
                "Assignment group", "Assigned to", 
                "Updated by", "Task type"
            };
            if (cell is null)
            {
                if (unassignedColumns.Contains(header))
                {
                    return ToCsvField("Unassigned"); 
                }
                return ""; 
            }

            string raw = cell.ToString()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(raw))
            {
                if (unassignedColumns.Contains(header))
                {
                   
                    return ToCsvField("Unassigned"); 
                }
                return ""; 
            }

            if (DateTime.TryParse(raw, out var dt))
            {
                string formatted = dt.ToString("MM-dd-yyyy HH:mm:ss");
                _logger.LogInformation("Date Converted | Header: {Header} | Original: {Original} -> New: {New}", header, raw, formatted);
                return ToCsvField(formatted);
            }
            return ToCsvField(raw);
        }
        private string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Removes spaces, casing, and special characters
            // e.g., "SLA due " becomes "sladue"
            return new string(input.ToLowerInvariant()
                                   .Where(c => char.IsLetterOrDigit(c))
                                   .ToArray());
        }
        private IExcelDataReader CreateExcelReader(Stream s, string ext)
        {
            return ext switch
            {
                ".xls"  => ExcelReaderFactory.CreateBinaryReader(s),
                ".xlsx" => ExcelReaderFactory.CreateOpenXmlReader(s),
                _       => throw new UnsupportedFormatException("Unsupported file extension.")
            };
        }

        private void ValidateHeaders(List<string> actual)
        {
            var expected = _ingestion.ExpectedHeaders ?? new List<string>();
            if (expected.Count == 0) return;

            var actualNorm   = actual.Select(h => (h ?? string.Empty).Trim().ToLowerInvariant()).ToList();
            var expectedNorm = expected.Select(h => (h ?? string.Empty).Trim().ToLowerInvariant()).ToList();

            if (_ingestion.RequireExactOrder)
            {
                var exact = actualNorm.Count == expectedNorm.Count &&
                            actualNorm.Zip(expectedNorm).All(p => p.First == p.Second);
                if (!exact)
                    throw new HeaderValidationException($"Exact header match required. Expected: [{string.Join(", ", expected)}]; Found: [{string.Join(", ", actual)}]");
                return;
            }

            var actualSet = actualNorm.ToHashSet();
            var missing = expected
                .Where((_, i) => !actualSet.Contains(expectedNorm[i]))
                .ToList();

            if (missing.Count > 0)
                throw new HeaderValidationException($"Missing required headers: [{string.Join(", ", missing)}]. Found: [{string.Join(", ", actual)}]");
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var safe = new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(safe) ? "upload.xls" : safe;
        }

        private static string ToCsvField(string s)
        {
            bool mustQuote = s.Contains('\"') || s.Contains(',') || s.Contains('\n') || s.Contains('\r');
            if (s.Contains('\"')) s = s.Replace("\"", "\"\"");
            return mustQuote ? $"\"{s}\"" : s;
        }
        public async Task<IEnumerable<UploadHistory>> GetAsync(UploadHistoryFilter filter, CancellationToken ct = default)
        {
            _logger.LogDebug(
                "Fetching UploadHistory: search={search}, sort={sortBy} {sortDir}, page={page}, size={size}",
                filter.SearchText, filter.SortBy, filter.SortDir, filter.PageNumber, filter.PageSize
            );

            var result = await _repo.GetAsync(filter, ct);
            return result;
        }

         public async Task<ImportSummary> ImportFromUploadAsync(int uploadHistoryId, CancellationToken ct = default)
        {
            if (uploadHistoryId <= 0) throw new ArgumentException("UploadHistoryId must be greater than zero.", nameof(uploadHistoryId));

            _logger.LogInformation("Starting import for UploadHistoryId {UploadHistoryId}", uploadHistoryId);

            var result = await _repo.ExecuteImportAsync(uploadHistoryId, ct);

            _logger.LogInformation("Import complete for UploadHistoryId {UploadHistoryId}: Inserted={Inserted} Updated={Updated} Skipped={Skipped}",
                uploadHistoryId, result.InsertedCount, result.UpdatedCount, result.SkippedDueToMissingNumber);

            return result;
        }
    }
}
