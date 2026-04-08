using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Services.Results;

public class Result
{
    public bool IsSuccess { get; }
    
    public IReadOnlyList<Error> Errors { get; }

    protected Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success() => new (true, []);

    public static Result Failure(params Error[] errors)
    {
        if(errors.Length is 0)
            throw new ArgumentException("At least one error must be provided", nameof(errors));
        
        return new Result(false, errors.AsReadOnly());
    }
    
    public static Result Failure(params IdentityError[] identityErrors)
    {
        if (identityErrors.Length == 0)
            throw new ArgumentException("At least one error must be provided within a failure");

        var errors = identityErrors.Select(e => new Error(e.Code, e.Description)).ToList();

        return new Result(false, errors.AsReadOnly());
    }
    
}

public class Result<T> : Result
{
    public T? Payload { get; }

    protected Result(bool isSuccess, IReadOnlyList<Error> errors) : base(isSuccess, errors)
    {
        Payload = default;
    }
    
    protected Result(bool isSuccess, IReadOnlyList<Error> errors, T? payload) : base(isSuccess, errors)
    {
        Payload = payload;
    }

    public static Result<T> Success(T? data)
    {
        if(data is null)
            throw new ArgumentNullException(nameof(data), "Payload cannot be null for a 'Successful' result with payload");
        
        return new Result<T>(true, [], data);
    }

    public new static Result<T> Failure(params Error[] errors)
    {
        if(errors.Length is 0)
            throw new ArgumentException("At least one error must be provided", nameof(errors));
        
        return new Result<T>(false, errors.AsReadOnly());
    }
    
    public new static Result<T> Failure(params IdentityError[] identityErrors)
    {
        if (identityErrors.Length == 0)
            throw new ArgumentException("At least one error must be provided within a failure");

        var errors = identityErrors.Select(e => new Error(e.Code, e.Description)).ToList();

        return new Result<T>(false, errors.AsReadOnly());
    }
    
}