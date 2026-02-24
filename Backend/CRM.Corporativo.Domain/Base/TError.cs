namespace CRM.Corporativo.Domain.Base;

public class TError : IEquatable<TError>
{
    public static readonly TError None = new(string.Empty, string.Empty);
    public static readonly TError NullValue = new("Result.Value","The specified result value is null.");

    public TError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public static implicit operator string(TError error) => error is null ? "" : error.Message;

    public virtual bool Equals(TError? other)
    {
        if (other is null)
        {
            return false;
        }

        return Message == other.Message;
    }

    public override bool Equals(object? obj) => obj is TError error && Equals(error);

    public override int GetHashCode() => HashCode.Combine(Message);

    public override string ToString() => Message;
}
