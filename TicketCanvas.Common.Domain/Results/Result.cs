using TicketCanvas.Common.Domain.Exceptions;

namespace TicketCanvas.Common.Domain.Results;

public class Result
{
    public bool IsSuccess => ErrorType == ErrorType.None;
    public ErrorType ErrorType { get; private set; }
    public string? ErrorMessage { get; private set; }

    protected Result()
    {
    }

    protected Result(ErrorType errorType, string? errorMessage)
    {
        ErrorType = errorType;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new();
    public static Result Failure(ErrorType errorType, string? errorMessage = null)
    {
        if (errorType == ErrorType.None)
            throw new DomainException("Error Type is required.");

        return new Result(errorType, errorMessage);
    }
}

public class Result<T> : Result
{
    private readonly T? _value;
    public T Value 
    { 
        get
        {
            if (_value == null)
                throw new DomainException("Value is null.");

            return _value;
        }
    }

    private Result(T value)
    {
        _value = value;
    }

    private Result(ErrorType errorType, string? errorMessage) : base(errorType, errorMessage)
    {
    }

    public static Result<T> Success(T value)
    {
        if (value == null)
            throw new DomainException("Value can not be null.");

        return new(value);
    }

    public static new Result<T> Failure(ErrorType errorType, string? errorMessage = null)
    {
        if (errorType == ErrorType.None)
            throw new DomainException("Error Type is required.");

        return new Result<T>(errorType, errorMessage);
    }
}