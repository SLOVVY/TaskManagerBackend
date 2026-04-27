using Domain.Abstractions;
using Domain.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        public readonly string[] allowedStatus = { "Открыта", "В работе", "Выполнена" };
        public readonly string[] allowedPriorities = { "Низкий", "Средний", "Высокий", "Критический" };
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<Result<List<TaskItem>>> GetAllTasksAsync(string status, string executor)
        {
            try
            {
                var tasks = await _taskRepository.GetAllTasksAsync(status, executor);

                return Result<List<TaskItem>>.Success(tasks);
            }
            catch (Exception ex)
            {
                return Result<List<TaskItem>>.Failure(
                    "Внутренняя ошибка сервера",
                    "INTERNAL_ERROR");
            }
        }

        public async Task<Result<TaskItem>> CreateTaskAsync(
            string name,
            string description,
            string priority,
            int hoursToDo,
            string executor,
            string status
            )
        {
            try
            {
                if (priority == allowedPriorities[3] && hoursToDo == 0)
                    hoursToDo = 24;

                (List<string> errors, var task) = TaskItem.CreateTask(
                    Guid.NewGuid(),
                    name,
                    description,
                    priority,
                    DateTime.UtcNow.AddHours(hoursToDo),
                    executor,
                    DateTime.UtcNow,
                    status,
                    null,
                    new List<Comment>()
                    );

                if (!allowedStatus.Contains(status))
                    errors.Add($"Недопустимый статус задачи. Разрешены: {string.Join(", ", allowedStatus)}");

                if (!allowedPriorities.Contains(priority))
                    errors.Add($"Недопустимый приоритет задачи. Разрешены: {string.Join(", ", allowedPriorities)}");

                if (errors.Count != 0)
                    return Result<TaskItem>.Failure(errors, "VALIDATION_ERROR");


                await _taskRepository.CreateTaskAsync(task);

                return Result<TaskItem>.Success(task);
            }
            catch (Exception ex)
            {
                return Result<TaskItem>.Failure(
                    "Внутренняя ошибка сервера",
                    "INTERNAL_ERROR");
            }
        }

        public async Task<Result<TaskItem>> GetTaskByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Result<TaskItem>.Failure(
                        "ID задачи не может быть пустым",
                        "VALIDATION_ERROR");
                }

                var task = await _taskRepository.GetTaskByIdAsync(id);

                if (task == null)
                {
                    return Result<TaskItem>.Failure(
                        $"Задача с ID = {id} не найдена",
                        "NOT_FOUND");
                }

                return Result<TaskItem>.Success(task);
            }
            catch (Exception ex)
            {
                return Result<TaskItem>.Failure(
                    "Внутренняя ошибка сервера",
                    "INTERNAL_ERROR");
            }
        }

        public async Task<Result<Guid>> UpdateTaskStatusAsync(Guid id, string newStatus)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(id);

                if (task == null)
                {
                    return Result<Guid>.Failure(
                        $"Задача с ID = {id} не найдена",
                        "NOT_FOUND");
                }

                if (!allowedStatus.Contains(newStatus))
                {
                    return Result<Guid>.Failure(
                        $"Недопустимый статус задачи. Разрешены: {string.Join(", ", allowedStatus)}",
                        "VALIDATION_ERROR");
                }

                if (await _taskRepository.GetAmountOfTasksOfExecutor(task.Executor) > 2 && newStatus == allowedStatus[1])
                    return Result<Guid>.Failure(
                        $"Исполнитель {task.Executor} уже выполняет более 2 задач, больше брать задачи нельзя",
                        "BUSINESS_RULE_VIOLAION");

                if (Array.IndexOf(allowedStatus, newStatus) - Array.IndexOf(allowedStatus, task.Status) > 1 ||
                    (Array.IndexOf(allowedStatus, newStatus) <= Array.IndexOf(allowedStatus, task.Status) && task.Priority == allowedPriorities[3]))
                {
                    return Result<Guid>.Failure(
                        "Недопустимый переход статусов: возможен только последовательный переход: открыта -> в работе, в работе -> выполнена. Критические задачи не могут быть переведены из статуса Выполнена в другой статус.",
                        "VALIDATION_ERROR");
                }

                await _taskRepository.UpdateStatusAsync(task.Id, newStatus);

                return Result<Guid>.Success(task.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(
                    "Внутренняя ошибка сервера",
                    "INTERNAL_ERROR");
            }
        }

        public async Task<Result<Guid>> DeleteTaskAsync(Guid id)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(id);

                if (task == null)
                {
                    return Result<Guid>.Failure(
                    $"Задача с ID {id} не найдена",
                    "NOT_FOUND");
                }

                await _taskRepository.DeleteTaskAsync(id);

                return Result<Guid>.Success(task.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(
                    "Внутренняя ошибка сервера",
                    "INTERNAL_ERROR");
            }
        }
    }
}
