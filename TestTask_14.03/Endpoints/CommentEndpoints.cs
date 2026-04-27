using Domain.Abstractions;
using TestTask_14._03.Contracts;

namespace TestTask_14._03.Endpoints
{
    public static class CommentEndpoints
    {
        public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comments")
                .WithTags("Comments")
                .WithOpenApi();

            group.MapPost("/", async (CreateCommentRequest request, ICommentService commentService) =>
            {
                var result = await commentService.CreateCommentAsync(
                    request.Text,
                    request.TaskId);

                if (result.IsFailure)
                    return Results.BadRequest(new { errors = result.Errors });

                return Results.Created($"/api/tasks/{result.Data.Id}", result.Data);
            })
            .WithName("CreateComment")
            .WithSummary("Создать новый комментарий");

            group.MapDelete("/{id:guid}", async (Guid id, ICommentService commentService) =>
            {
                var result = await commentService.DeleteCommentAsync(id);

                if (result.IsFailure)
                    return Results.BadRequest(new { errors = result.Errors });

                return Results.Ok(id);
            })
            .WithName("DeleteComment")
            .WithSummary("Удалить комментарий");

            return app;
        }
    }
}
