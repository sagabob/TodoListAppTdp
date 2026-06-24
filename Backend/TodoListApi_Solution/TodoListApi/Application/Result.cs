namespace TodoListApi.Application;

public class Result<T> where T : class
{
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = error;
    }

    public bool IsSuccess { get; init; }

    public T? Value { get; init; }

    public string? ErrorMessage { get; init; }
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, null, errorMessage);
    }
}

public class Result
{
    public bool IsSuccess { get; init; }

    public string? ErrorMessage { get; init; }

    public static Result Success() =>
        new() { IsSuccess = true };

    public static Result Failure(string errorMessage) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage };
}