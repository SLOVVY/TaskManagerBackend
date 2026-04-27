using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class CommentEntity
    {
        public required Guid Id { get; set; }

        [MaxLength(500)]
        public required string Text { get; set; } = "[Пустой комментарий]";
        public required DateTime CreatedAt { get; set; }


        public required Guid TaskId { get; set; }
        public TaskEntity Task { get; set; } = null!;
    }
}
