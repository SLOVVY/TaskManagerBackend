namespace Domain.Models
{
    public class TaskItem
    {
        private const int MIN_TASK_NAME_LENGTH = 5;
        private const int MAX_TASK_NAME_LENGTH = 200;
        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; } = string.Empty;
        public string Priority { get; }
        public DateTime ExpiredDate { get; }
        public string Executor { get; }
        public DateTime CreatedAt { get; }
        public string Status { get; } = "Открыта";
        public DateTime? ExecutedAt { get; }
        public List<Comment> Comments { get; } = [];
        public bool IsExpired =>
            Status != "Выполнена" &&
            DateTime.UtcNow > ExpiredDate;

        private TaskItem(
            Guid id,
            string name,
            string description,
            string priority,
            DateTime expiredDate,
            string executor,
            DateTime createdAt,
            string status,
            DateTime? executedAt,
            List<Comment> comments
            )
        {
            Id = id;
            Name = name;
            Description = description;
            Priority = priority;
            ExpiredDate = expiredDate;
            Executor = executor;
            CreatedAt = createdAt;
            Status = status;
            ExecutedAt = executedAt;
            Comments = comments;
        }

        public static (List<string> errors, TaskItem task) CreateTask(
            Guid id,
            string name,
            string description,
            string priority,
            DateTime expiredDate,
            string executor,
            DateTime createdAt,
            string status,
            DateTime? executedAt,
            List<Comment> comments)
        {
            List<string> errors = [];

            if (name.Length < MIN_TASK_NAME_LENGTH || name.Length > MAX_TASK_NAME_LENGTH)
                errors.Add("Название задачи не может быть меньше 5 символов в длину и не должен превышать 200 символов в длину");

            if (string.IsNullOrEmpty(priority))
                errors.Add("Нельзя указать пустой приоритет");

            if (DateTime.MinValue == expiredDate || expiredDate < DateTime.UtcNow)
                errors.Add("Нельзя установить пустую дату выполнения или меньше текущей");

            if (string.IsNullOrEmpty(executor))
                errors.Add("Исполнитель должен быть назначен");

            TaskItem task = new TaskItem(
            id,
            name,
            description,
            priority,
            expiredDate,
            executor,
            createdAt,
            status,
            executedAt,
            comments);

            return (errors, task);
        }
    }
}