using Domain.Models;

namespace Domain.Abstractions
{
    public interface ICommentRepository
    {
        Task<Guid> CreateCommentAsync(Comment comment);
        Task<Guid> DeleteCommentAsync(Guid id);
        Task<Comment> GetCommentById(Guid id);
    }
}