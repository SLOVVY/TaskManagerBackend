using Microsoft.AspNetCore.Mvc;

namespace Domain.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public List<string> Errors { get; } = [];
        public T Data { get; }

        private Result(bool isSuccess, T data, List<string> errors)
        {
            IsSuccess = isSuccess;
            Data = data;
            Errors = errors ?? [];
        }

        public static Result<T> Success(T data) => new(true, data, null!);
        public static Result<T> Failure(List<string> errors, string errorDefinition) => new(false, default!, errors);
        public static Result<T> Failure(string error, string errorDefinition) => new(false, default!, [error]);

        public IActionResult ToActionResult(Func<T, IActionResult> onSuccess)
        {
            if (IsSuccess)
                return onSuccess(Data);

            return new BadRequestObjectResult(new { Errors });
        }
    }
}
