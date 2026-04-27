namespace Domain.Models
{
    public record AnalyticsDTOs
    {
        public int TotalTasks { get; init; }
        public List<StatusCountDto> TasksByStatus { get; init; } = new();
        public int ExpiredTasks { get; init; }
        public double AverageCompletionDays { get; init; }
        public string? TopExpiredPerformer { get; init; }
        public int TopExpiredCount { get; init; }
    }
    public record StatusCountDto
    {
        public string Status { get; init; } = string.Empty;
        public int Count { get; init; }
    }
}
