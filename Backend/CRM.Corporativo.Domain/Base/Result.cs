namespace CRM.Corporativo.Domain.Base;

public class Result
{
    protected internal Result(bool success, List<TError> errors = null)
    {
        IsSuccess = success;
        Errors = errors ?? new List<TError>();
    }

    protected internal Result(bool success, TError error)
    {
        IsSuccess = success;
        Errors = new List<TError> { error };
    }

    public bool IsSuccess { get; }
    public List<TError> Errors { get; }

    public static Result Success() => new(true, TError.None);

    public static Result Fail(TError error) => new(false, error);

    public static Result Fail(List<TError> errors) => new(false, errors);

    public static Result Fail(string errorCode, string errorMessage) =>
        new(false, new TError(errorCode, errorMessage));

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, TError.None);

    public static Result<TValue> Fail<TValue>(TError error) => new(default, false, error);

    public static Result<TValue> Fail<TValue>(List<TError> errors) => new(default, false, errors);

    public static Result<TValue> Fail<TValue>(string errorCode, string errorMessage) =>
        new(default, false, new TError(errorCode, errorMessage));

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Fail<TValue>(TError.NullValue);
}

public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T? value, bool isSuccess, List<TError> errors = null)
        : base(isSuccess, errors) =>
        _value = value;

    protected internal Result(T? value, bool isSuccess, TError error)
        : base(isSuccess, error) =>
        _value = value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    public static implicit operator Result<T>(T? value) => Create(value);

    public Result<T> ToResult(T? value) => Create(value);
}
