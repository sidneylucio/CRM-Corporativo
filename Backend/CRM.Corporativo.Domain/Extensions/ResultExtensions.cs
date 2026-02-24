using CRM.Corporativo.Domain.Base;

namespace CRM.Corporativo.Domain.Extensions;

public static class ResultExtensions
{
    public static T Match<T, TValue>(
        this Result<TValue> result,
        Func<TValue, T> onSuccess,
        Func<Result, T> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(Result.Fail(result.Errors));
    }

    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<Result, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(Result.Fail(result.Errors));
    }
}
