using FlowerShop.SharedKernel.Results;

namespace FlowerShop.SharedKernel.Extensions;

public static class ResultExtensions
{
    public static Result HandleResult(this Result result)
    {
        if (result.IsSuccess)
            return Result.Success();

        return Result.Failure(result.Errors.ToArray());
    }

    public static Result<T> HandleResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return result.Payload is null ? Result<T>.Success() : Result<T>.Success(result.Payload);

        return Result<T>.Failure(result.Errors.ToArray());
    }
}