using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class TaskEntity
    {
        public required Guid Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public required string Priority { get; set; }

        public required DateTime ExpiredDate { get; set; }

        public required string Executor { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required string Status { get; set; }
        public required DateTime? ExecutedAt { get; set; }

        
        public List<CommentEntity> CommentsEntities { get; set; } = new List<CommentEntity>();
    }
}
