using Domain.Models;

namespace Domain.Abstractions
{
    public interface ITaskService
    {
        Task<Result<TaskItem>> CreateTaskAsync(string name, string description, string priority, int hoursToDo, string executor, string status);
        Task<Result<Guid>> DeleteTaskAsync(Guid id);
        Task<Result<List<TaskItem>>> GetAllTasksAsync(string status, string executor);
        Task<Result<TaskItem>> GetTaskByIdAsync(Guid id);
        Task<Result<Guid>> UpdateTaskStatusAsync(Guid id, string newStatus);
    }
}