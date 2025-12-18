using Incident.Application.Dtos.Requests;
using Incident.Application.Dtos.Responses;
using Incident.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace IncidentAPI.Mappers
{
    public static class IncidentMappingExtensions
    {
        public static IncidentFilter ToDomainFilter(this DashboardFilterRequest request)
        {
            var filter = new IncidentFilter
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                AssignmentGroup = request.AssignmentGroup,
                Category = request.Category,
                Priority = request.Priority,
                AssignedToName = request.AssignedToName,
                State = request.State
            };

            // Handle Pagination/Sorting if it's the paginated request type
            if (request is DashboardFilterPaginatedRequest paginated)
            {
                filter.PageNumber = paginated.PageNumber;
                filter.PageSize = paginated.PageSize;
                filter.SortBy = paginated.SortBy;
                filter.SortOrder = paginated.SortOrder;
                filter.Search = paginated.Search;

                // Specific fields for BreachList filtering
                filter.IncidentNumber = paginated.IncidentNumber;
                filter.ActualResolvedTime = paginated.ActualResolvedTime;
                filter.BreachSLA = filter.BreachSLA = paginated.BreachSLA;
            }

            return filter;
        }
        public static DashboardKpiResponse ToResponse(this DashboardKpi kpi)
        {
            if (kpi == null) return new DashboardKpiResponse();
            return new DashboardKpiResponse
            {
                TotalIncidents = kpi.TotalIncidents,
                OpenCount = kpi.Open_Count,
                BreachedCount = kpi.Breached_Count,
                OpenMore15Days = kpi.Open_More_15_Days,
                OpenLess15Days = kpi.Open_Less_15_Days,
                States = kpi.StateCounts
            };
        }

        public static MemberIncidentStatsResponse ToResponse(this PagedMemberIncidentStats result)
        {
            return new MemberIncidentStatsResponse
            {
                MemberDetails = result?.MemberDetails?.Select(x => new NameAndIncidentCountByPriorityResponse
                {
                    Name = x.Name,
                    P1 = x.P1,
                    P2 = x.P2,
                    P3 = x.P3,
                    P4 = x.P4,
                    TotalCount = x.TotalCount,
                    ActualResolvedTime = x.ActualResolvedTime,
                    LastUpdated = x.LastUpdated
                }).ToList() ?? new List<NameAndIncidentCountByPriorityResponse>(),

                Pagination = new PaginationResponse
                {
                    Page = result?.Pagination?.Page ?? 1,
                    PageSize = result?.Pagination?.PageSize ?? 0,
                    TotalRecords = result?.Pagination?.TotalRecords ?? 0,
                    TotalPages = result?.Pagination?.TotalPages ?? 0,
                    SortBy = result?.Pagination?.SortBy ?? string.Empty,
                    SortOrder = result?.Pagination?.SortOrder ?? string.Empty
                }
            };
        }

        public static List<AssignmentGroupResponse> ToResponse(this IEnumerable<AssignmentGroup> groups)
        {
            if (groups == null || !groups.Any()) return new List<AssignmentGroupResponse>();

            return groups.Select(r => new AssignmentGroupResponse
            {
                AssignmentGroupName = r.AssignmentGroupName,
                IncidentCount = r.IncidentCount
            }).ToList();
        }

        public static List<StatusCountByPriorityResponse> ToResponse(this IEnumerable<StatusCountByPriority> statuses)
        {
            if (statuses == null || !statuses.Any()) return new List<StatusCountByPriorityResponse>();

            return statuses.Select(r => new StatusCountByPriorityResponse
            {
                Status = r.Status,
                IncidentCount = r.IncidentCount
            }).ToList();
        }

        public static List<CategoryCountByGroupResponse> ToResponse(this IEnumerable<CategoryCountByGroup> categories)
        {
            if (categories == null || !categories.Any()) return new List<CategoryCountByGroupResponse>();

            return categories.Select(r => new CategoryCountByGroupResponse
            {
                CategoryName = r.CategoryName,
                IncidentCount = r.IncidentCount
            }).ToList();
        }

        public static IncidentCountByPriorityGroupedResponse ToResponse(this IEnumerable<IncidentCountByPriority> counts)
        {
            var response = new IncidentCountByPriorityGroupedResponse();

            if (counts == null || !counts.Any()) return response;

            // Group by Priority (P1, P2, etc.)
            foreach (var priorityGroup in counts.GroupBy(r => r.Priority))
            {
                // Use the first record of the group for common priority-level metrics
                var first = priorityGroup.First();

                var data = new PriorityData
                {
                    TotalCountForPriority = (int)first.TotalCount,
                    AvgResolvedTime = first.AvgResolvedTime,
                    TotalResolvedTime = first.TotalResolvedTime,
                    // Summing metrics that might exist across multiple state rows for this priority
                    BreachedCount = priorityGroup.Sum(g => g.BreachedCount),
                    OpenMoreThan15Days = priorityGroup.Sum(g => g.Open_more_15_days),
                    OpenLessThan15Days = priorityGroup.Sum(g => g.Open_less_15_days)
                };

                // AUTOMATIC HANDLING: Map every state returned by the DB
                foreach (var item in priorityGroup)
                {
                    if (!string.IsNullOrEmpty(item.State))
                    {
                        data.StateDetails[item.State] = item.IncidentCount;
                    }
                }

                response.Priority[priorityGroup.Key] = data;
            }

            return response;
        }

        public static IncidentDetailsPaginatedResponse ToResponse(this IEnumerable<IncidentDetailsByPriority> incidents)
        {
            var response = new IncidentDetailsPaginatedResponse
            {
                PageNumber = incidents.FirstOrDefault()?.PageNumber ?? 1,
                PageSize = incidents.FirstOrDefault()?.PageSize ?? 8,
                TotalPages = incidents.FirstOrDefault()?.TotalPages ?? 0,
                TotalElements = incidents.FirstOrDefault()?.TotalElements ?? 0,
                Incidents = incidents.Select(x => new IncidentDetailsResponse
                {
                    IncidentNo = x.IncidentNo,
                    AssignedTo = x.AssignedTo,
                    ShortDescription = x.ShortDescription,
                    Category = x.Category,
                    State = x.State,
                    CreatedDateTime = x.Created,
                    UpdatedDateTime = x.Updated,
                    ResolvedDateTime = x.ResolvedDateTime,
                    ActualResolvedTime = x.ActualResolvedTime,
                    BreachSLA = x.BreachSLA
                }).ToList()
            };
            return response;
        }

        public static BreachListPagedResponse ToResponse(this IEnumerable<BreachListItem> items)
        {
            return new BreachListPagedResponse
            {
                PageNumber = items.FirstOrDefault()?.PageNumber ?? 1,
                PageSize = items.FirstOrDefault()?.PageSize ?? 8,
                TotalPages = items.FirstOrDefault()?.TotalPages ?? 0,
                TotalElements = items.FirstOrDefault()?.TotalElements ?? 0,
                Items = items.Select(i => new BreachListItemResponse
                {
                    IncidentNumber = i.IncidentNumber,
                    AssignedTo = i.AssignedTo,
                    ShortDescription = i.ShortDescription,
                    Category = i.Category,
                    State = i.State,
                    CreatedDateTime = i.Created,
                    UpdatedDateTime = i.Updated,
                    ResolvedDateTime = i.ResolvedDateTime,
                    ActualResolvedTime = i.ActualResolvedTime,
                    BreachSLA = i.BreachSLA
                }).ToList()
            };
        }
    }
}