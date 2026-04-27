namespace Domain.Models
{
    public class Comment
    {
        private const int MAX_COMMENT_NAME_LENGTH = 500;
        public Guid Id { get; }
        public string Text { get; }
        public Guid TaskId { get; }
        public DateTime CreatedAt { get; }

        private Comment(
            Guid id,
            string text,
            Guid taskId,
            DateTime createdAt
            )
        {
            Id = id;
            Text = text;
            TaskId = taskId;
            CreatedAt = createdAt;
        }

        public static (List<string> errors, Comment comment) CreateComment(
            Guid id,
            string text,
            Guid taskId,
            DateTime createdAt)
        {
            List<string> errors = [];

            if (string.IsNullOrEmpty(text) || text.Length >= MAX_COMMENT_NAME_LENGTH)
                errors.Add("Текст комментария не может быть пустым и должен быть не более 500 символов в длину");

            Comment comment = new Comment(
            id,
            text,
            taskId,
            createdAt);

            return (errors, comment);
        }
    }
}
