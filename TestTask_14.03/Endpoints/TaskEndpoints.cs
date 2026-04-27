using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TestTask_14._03.Contracts;

namespace TestTask_14._03.Endpoints
{
    public static class TaskEndpoints
    {
        public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/tasks")
                .WithTags("Tasks")
                .WithOpenApi();

            group.MapGet("/", async (ITaskService taskService, [FromQuery] string? status, [FromQuery] string? executor) =>
            {
                var tasks = await taskService.GetAllTasksAsync(status ?? "Все", executor ?? "Все");

                var response = tasks.Data.Select(task => new GetTaskResponse(
                    task.Id,
                    task.Name,
                    task.Description,
                    task.Priority,
                    task.ExpiredDate,
                    task.Executor,
                    task.CreatedAt,
                    task.Status,
                    task.ExecutedAt,
                    task.Comments?.Select(c => new GetCommentResponse(
                        c.Id,
                        c.Text,
                        c.CreatedAt,
                        c.TaskId
                    )).ToList(),
                    task.IsExpired
                ));

                return Results.Ok(response);
            })
            .WithName("GetAllTasks")
            .WithSummary("Получить все задачи")
            .WithDescription("Возвращает список задач с возможностью фильтрации по статусу и испольнителю");

            group.MapGet("/{id:guid}", async (ITaskService taskService, Guid id) =>
            {
                var result = await taskService.GetTaskByIdAsync(id);

                if (result == null)
                    return Results.NotFound();

                var task = result.Data; 

                var response = new GetTaskResponse(
                    task.Id,
                    task.Name,
                    task.Description,
                    task.Priority,
                    task.ExpiredDate,
                    task.Executor,
                    task.CreatedAt,
                    task.Status,
                    task.ExecutedAt,
                    task.Comments?.Select(c => new GetCommentResponse(
                        c.Id,
                        c.Text,
                        c.CreatedAt,
                        c.TaskId
                    )).ToList(),
                    task.IsExpired
                );

                return Results.Ok(response);
            })
            .WithName("GetTaskById")
            .WithSummary("Получить задачу по ID");

            group.MapPost("/", async (CreateTaskRequest request, ITaskService taskService) =>
            {
                var result = await taskService.CreateTaskAsync(
                    request.Name,
                    request.Description,
                    request.Priority,
                    request.HoursToDo,
                    request.Executor,
                    request.Status);

                if (result.IsFailure)
                    return Results.BadRequest(new { errors = result.Errors });

                return Results.Created($"/api/tasks/{result.Data.Id}", result.Data);
            })
            .WithName("CreateTask")
            .WithSummary("Создать новую задачу");

            group.MapPatch("/{id:guid}", async (Guid id, UpdateTaskStatusRequest request, ITaskService taskService) =>
            {
                var result = await taskService.UpdateTaskStatusAsync(id, request.Status);

                if (result.IsFailure)
                    return Results.BadRequest(new { errors = result.Errors });

                return Results.Ok($"Статус задачи {id} успешно обновлён");
            })
            .WithName("UpdateTaskStatus")
            .WithSummary("Обновить статус задачи");

            group.MapDelete("/{id:guid}", async (Guid id, ITaskService taskService) =>
            {
                var result = await taskService.DeleteTaskAsync(id);

                if (result.IsFailure)
                    return Results.BadRequest(new {errors = result.Errors});

                return Results.Ok($"Задача {id} успешно удалена");
            })
            .WithName("DeleteTask")
            .WithSummary("Удалить задачу");

            return app;
        }
    }
}
