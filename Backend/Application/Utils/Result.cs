using Application.Constants.ApiErrors;

namespace Domain.Shared;

public class Result<T>
{
    private readonly T? _data;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    // Ensures we never access Data on failure
    public T Data => IsSuccess 
        ? _data! 
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    private Result(bool isSuccess, Error error, T? value)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        _data = value;
    }

    // Factory Method for Success
    public static Result<T> Success(T data) => new(true, Error.None, data);

    // Factory Method for Failure
    public static Result<T> Failure(Error error) => new(false, error, default);
}