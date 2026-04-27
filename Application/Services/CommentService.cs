using Domain.Abstractions;
using Domain.Models;

namespace Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<Result<Comment>> CreateCommentAsync(string text, Guid taskId)
        {
            try
            {
                (List<string> errors, var comment) = Comment.CreateComment(
                    Guid.NewGuid(),
                    text,
                    taskId,
                    DateTime.UtcNow);

                if (errors.Count != 0)
                    return Result<Comment>.Failure(errors, "VALIDATION_ERROR");

                await _commentRepository.CreateCommentAsync(comment);

                return Result<Comment>.Success(comment);
            }
            catch (Exception ex)
            {
                return Result<Comment>.Failure(
                    "Внутренняя ошибка сервера",
                    "INTERNAL_ERROR");
            }

        }

        public async Task<Result<Guid>> DeleteCommentAsync(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentById(id);

                if (comment == null)
                {
                    return Result<Guid>.Failure(
                        $"Задача с ID = {id} не найдена",
                        "NOT_FOUND");
                }

                await _commentRepository.DeleteCommentAsync(id);

                return Result<Guid>.Success(comment.Id);
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
