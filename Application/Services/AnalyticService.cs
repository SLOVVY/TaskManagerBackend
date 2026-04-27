using Domain.Abstractions;
using Domain.Models;

namespace Application.Services
{
    public class AnalyticService : IAnalyticService
    {
        private readonly ITaskRepository _taskRepository;

        public AnalyticService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<Result<AnalyticsDTOs>> GetAnalyticsAsync()
        {
            try
            {
                var totalTasksAmount = _taskRepository.GetTotalTasksCountAsync();
                var tasksByStatusTask = _taskRepository.GetTasksCountByStatusAsync();
                var expiredTasksAmount = _taskRepository.GetExpiredTasksCountAsync();
                var averageCompletionTask = _taskRepository.GetAverageCompletionDaysAsync();
                var topExpiredTaskPerformer = _taskRepository.GetTopExpiredPerformerAsync();

                await Task.WhenAll(
                    totalTasksAmount,
                    tasksByStatusTask,
                    expiredTasksAmount,
                    averageCompletionTask,
                    topExpiredTaskPerformer
                );

                var tasksByStatus = (await tasksByStatusTask)
                    .Select(kv => new StatusCountDto
                    {
                        Status = kv.Key,
                        Count = kv.Value
                    })
                    .OrderBy(x => x.Status)
                    .ToList();

                var (topPerformer, topCount) = await topExpiredTaskPerformer;

                var analyticDTO = new AnalyticsDTOs
                {
                    TotalTasks = await totalTasksAmount,
                    TasksByStatus = tasksByStatus,
                    ExpiredTasks = await expiredTasksAmount,
                    AverageCompletionDays = await averageCompletionTask,
                    TopExpiredPerformer = topPerformer,
                    TopExpiredCount = topCount
                };

                return Result<AnalyticsDTOs>.Success(analyticDTO);
            }
            catch (Exception ex)
            {
                return Result<AnalyticsDTOs>.Failure(
                    "Ошибка сбора данных",
                    "INTERNAL_ERROR");
            }
        }
    }
}
