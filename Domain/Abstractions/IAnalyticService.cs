using Domain.Models;

namespace Domain.Abstractions
{
    public interface IAnalyticService
    {
        Task<Result<AnalyticsDTOs>> GetAnalyticsAsync();
    }
}