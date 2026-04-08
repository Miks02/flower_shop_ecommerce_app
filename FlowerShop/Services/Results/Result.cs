namespace FlowerShop.Services.Results;

public class Result
{
    public bool IsSucceeded { get; }
    
    public IReadOnlyList<Error>? Errors { get; }

    protected Result(bool isSucceeded, IReadOnlyList<Error>? errors)
    {
        IsSucceeded = isSucceeded;
        Errors = errors;
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(params Error[] errors)
    {
        if(errors.Length == 0)
            throw new ArgumentException("At least one error must be provided", nameof(errors));
        
        return new Result(false, errors.AsReadOnly());
    }
    
}

public class Result<T> : Result
{
    public T? Payload { get; }

    protected Result(bool isSucceeded, IReadOnlyList<Error>? errors) : base(isSucceeded, errors)
    {
        Payload = default;
    }
    
    protected Result(bool isSucceeded, IReadOnlyList<Error>? errors, T? payload) : base(isSucceeded, errors)
    {
        Payload = payload;
    }

    public static Result<T> Success(T? data)
    {
        if(data is null)
            throw new ArgumentNullException(nameof(data),"Payload cannot be null for a Success result with payload");
        
        return new Result<T>(true, null, data);
    }

    public new static Result<T> Failure(params Error[] errors)
    {
        if(errors.Length == 0)
            throw new ArgumentException("At least one error must be provided", nameof(errors));
        
        return new Result<T>(false, errors.AsReadOnly());
    }
    
}