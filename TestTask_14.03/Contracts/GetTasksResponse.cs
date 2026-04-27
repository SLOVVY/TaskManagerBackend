namespace TestTask_14._03.Contracts
{
    public record GetTaskResponse(
    Guid Id,
    string Name,
    string? Description,
    string Priority,
    DateTime? ExpiredDate,
    string Executor,
    DateTime CreatedAt,
    string Status,
    DateTime? ExecutedAt,
    List<GetCommentResponse>? Comments,
    bool IsExpired
);

    public record GetCommentResponse(
        Guid Id,
        string Text,
        DateTime CreatedAt,
        Guid TaskId
    );
}
