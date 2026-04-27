using Domain.Models;

namespace Domain.Abstractions
{
    public interface ITaskRepository
    {
        Task<Guid> CreateTaskAsync(TaskItem task);
        Task<Guid> DeleteTaskAsync(Guid id);
        Task<List<TaskItem>> GetAllTasksAsync(string status, string executor);
        Task<TaskItem> GetTaskByIdAsync(Guid id);
        Task<Guid> UpdateStatusAsync(Guid id, string status);
        Task<int> GetAmountOfTasksOfExecutor(string executor);

        // Методы для аналитики //

        public Task<int> GetTotalTasksCountAsync();
        public Task<Dictionary<string, int>> GetTasksCountByStatusAsync();
        public Task<int> GetExpiredTasksCountAsync();
        public Task<double> GetAverageCompletionDaysAsync();
        public Task<(string? Executor, int Count)> GetTopExpiredPerformerAsync();

    }
}