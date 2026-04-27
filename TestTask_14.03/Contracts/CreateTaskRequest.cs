using System.ComponentModel.DataAnnotations;

namespace TestTask_14._03.Contracts
{
    public record CreateTaskRequest
    {
        required public string Name { get; init; }
        required public string Description { get; init; }
        required public string Priority { get; init; }
        required public int HoursToDo { get; init; }
        required public string Executor { get; init; }
        required public string Status { get; init; }
    }
}
