namespace TestTask_14._03.Contracts
{
    public record CreateCommentRequest
    {
        required public string Text { get; init; }
        required public Guid TaskId { get; init; }
    }
}
