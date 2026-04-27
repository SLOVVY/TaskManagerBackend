using Data.Entities;
using Domain.Models;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _dbContext;

        public TaskRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TaskItem>> GetAllTasksAsync(string status, string executor)
        {
            var query = _dbContext.Tasks
                .OrderBy(x => x.CreatedAt)
                .AsNoTracking();

            query = (status != "Все" && status != "Просроченные")
                ? query.Where(x => x.Status == status)
                : query;

            query = (status == "Просроченные")
                ? query.Where(x => x.ExpiredDate < DateTime.UtcNow)
                : query;

            query = (executor != "Все")
                ? query.Where(x => x.Executor == executor)
                : query;

            var taskEntities = await query.ToListAsync();

            var tasks = taskEntities
                .Select(t => TaskItem.CreateTask(
                    t.Id,
                    t.Name,
                    t.Description,
                    t.Priority,
                    t.ExpiredDate,
                    t.Executor,
                    t.CreatedAt,
                    t.Status,
                    t.ExecutedAt,
                    new List<Comment>()).task).ToList();

            return tasks;
        }

        public async Task<int> GetAmountOfTasksOfExecutor(string executor)
        {
            var amountOfTasks = await _dbContext.Tasks
                .Where(x => x.Executor == executor && x.Status == "В работе")
                .CountAsync();

            return amountOfTasks;
        }

        public async Task<TaskItem> GetTaskByIdAsync(Guid id)
        {
            var taskEntity = await _dbContext.Tasks
                .Include(x => x.CommentsEntities)
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (taskEntity == null)
                return null!;

            var comments = taskEntity.CommentsEntities?
                .Select(c => Comment.CreateComment(c.Id, c.Text, c.TaskId, c.CreatedAt).comment)
                .ToList() ?? new List<Comment>();

            var task = TaskItem.CreateTask(
                taskEntity.Id,
                taskEntity.Name,
                taskEntity.Description,
                taskEntity.Priority,
                taskEntity.ExpiredDate,
                taskEntity.Executor,
                taskEntity.CreatedAt,
                taskEntity.Status,
                taskEntity.ExecutedAt,
                comments).task;

            return task;
        }

        public async Task<Guid> CreateTaskAsync(TaskItem task)
        {
            var taskEntity = new TaskEntity
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                Priority = task.Priority,
                ExpiredDate = task.ExpiredDate,
                Executor = task.Executor,
                CreatedAt = task.CreatedAt,
                Status = task.Status,
                ExecutedAt = task.ExecutedAt,
            };

            await _dbContext.AddAsync(taskEntity);
            await _dbContext.SaveChangesAsync();

            return taskEntity.Id;
        }

        public async Task<Guid> UpdateStatusAsync(Guid id, string status)
        {
            await _dbContext.Tasks
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Status, status)
                .SetProperty(x => x.ExecutedAt,
                    status == "Выполнена" ? DateTime.UtcNow : null)
                );

            return id;
        }

        public async Task<Guid> DeleteTaskAsync(Guid id)
        {
            await _dbContext.Tasks
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Методы для аналитики //

        public async Task<int> GetTotalTasksCountAsync()
        {
            return await _dbContext.Tasks.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetTasksCountByStatusAsync()
        {
            return await _dbContext.Tasks
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<int> GetExpiredTasksCountAsync()
        {
            var now = DateTime.UtcNow;

            return await _dbContext.Tasks
                .Where(t => t.Status != "Выполнена" &&
                            t.ExpiredDate < now)
                .CountAsync();
        }

        public async Task<double> GetAverageCompletionDaysAsync()
        {
            var completedTasks = await _dbContext.Tasks
                .Where(t => t.Status == "Выполнена" && t.ExecutedAt != null)
                .Select(t => new
                {
                    CreatedAt = t.CreatedAt,
                    ExecutedAt = t.ExecutedAt.Value
                })
                .ToListAsync();

            if (!completedTasks.Any())
                return 0;

            var totalDays = completedTasks.Sum(t => (t.ExecutedAt - t.CreatedAt).TotalDays);
            var averageDays = totalDays / completedTasks.Count;

            return Math.Round(averageDays, 2);
        }

        public async Task<(string? Executor, int Count)> GetTopExpiredPerformerAsync()
        {
            var now = DateTime.UtcNow;

            var topPerformer = await _dbContext.Tasks
                .Where(t => t.Status != "Выполнена" &&
                            t.ExpiredDate < now &&
                            t.Executor != null)
                .GroupBy(t => t.Executor)
                .Select(g => new { Executor = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            return topPerformer != null
                ? (topPerformer.Executor, topPerformer.Count)
                : (null, 0);
        }
    }
}