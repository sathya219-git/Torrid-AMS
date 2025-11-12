namespace Incident.Domain.Models
{
    public class ImportSummary
    {
        public int StagingRowCount { get; set; }
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int MatchedButNotUpdatedCount { get; set; }
        public int SkippedDueToMissingNumber { get; set; }
    }
}
