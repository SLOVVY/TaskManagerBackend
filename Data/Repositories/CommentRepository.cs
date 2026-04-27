using Data.Entities;
using Domain.Models;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _dbContext;

        public CommentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateCommentAsync(Comment comment)
        {
            var commentEntity = new CommentEntity
            {
                Id = comment.Id,
                Text = comment.Text,
                TaskId = comment.TaskId,
                CreatedAt = comment.CreatedAt,
            };

            await _dbContext.AddAsync(commentEntity);
            await _dbContext.SaveChangesAsync();

            return commentEntity.Id;
        }

        public async Task<Comment> GetCommentById(Guid id)
        {
            var commentEntity = await _dbContext.Comments
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (commentEntity == null)
                return null!;

            var comment = Comment.CreateComment(
                commentEntity.Id,
                commentEntity.Text,
                commentEntity.TaskId,
                commentEntity.CreatedAt).comment;

            return comment;
        }

        public async Task<Guid> DeleteCommentAsync(Guid id)
        {
            await _dbContext.Comments
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
