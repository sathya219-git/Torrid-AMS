using Incident.Application.Dtos.Requests;
using Incident.Application.Dtos.Responses;
using Incident.Application.Helpers; 
using Incident.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IncidentAPI.Mappers
{
    public static class FileMappingExtensions
    {
        public static UploadHistoryFilter ToDomainFilter(this UploadHistoryQueryRequest request)
        {
            return new UploadHistoryFilter
            {
                SearchText = request.SearchText,
                SortBy = request.SortBy,
                SortDir = request.SortDir,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public static FileIngestionResponse ToResponse(this FileIngestionResult result)
        {
            return new FileIngestionResponse
            {
                UploadID = result.UploadID,
                OriginalFileName = result.OriginalFileName,
                FileSizeBytes = result.FileSizeBytes,
                CsvPath = result.CsvPath,
                Message = result.Message
            };
        }

        public static PagedUploadHistoryResponse ToPagedResponse(this List<UploadHistory> rows, UploadHistoryFilter filter)
        {
            var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

            var totalPages = filter.PageSize > 0
                ? (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                : 0;

            return new PagedUploadHistoryResponse
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                Items = rows.Select(r => new UploadHistoryItemResponse
                {
                    ID = r.ID,
                    FileName = r.FileName,
                    FileSize = FilterHelper.FormatSize(r.FileSize).ToString(),
                    UploadedDate = r.Uploaded_At
                }).ToList()
            };
        }

        public static ImportSummaryResponse ToResponse(this ImportSummary summary)
        {
            return new ImportSummaryResponse
            {
                StagingRowCount = summary.StagingRowCount,
                InsertedCount = summary.InsertedCount,
                UpdatedCount = summary.UpdatedCount,
                MatchedButNotUpdatedCount = summary.MatchedButNotUpdatedCount,
                SkippedDueToMissingNumber = summary.SkippedDueToMissingNumber
            };
        }
    }
}