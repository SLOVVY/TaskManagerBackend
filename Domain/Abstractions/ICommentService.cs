using Domain.Models;

namespace Domain.Abstractions
{
    public interface ICommentService
    {
        Task<Result<Comment>> CreateCommentAsync(string text, Guid taskId);
        Task<Result<Guid>> DeleteCommentAsync(Guid id);
    }
}