namespace FlowerShop.Services.Results;

public class ServiceResult
{
    public bool IsSucceeded { get; }
    
    public bool IsFailure => !IsSucceeded;
    
    public IReadOnlyList<Error>? Errors { get; }

    protected ServiceResult(bool isSucceeded, IReadOnlyList<Error>? errors)
    {
        IsSucceeded = isSucceeded;
        Errors = errors;
    }

    public static ServiceResult Success()
    {
        return new ServiceResult(true, null);
    }

    public static ServiceResult Failure(params Error[] errors)
    {
        if(errors.Length == 0)
            throw new ArgumentException("At least one error must be provided", nameof(errors));
        
        return new ServiceResult(false, errors.AsReadOnly());
    }
    
}

public class ServiceResult<T> : ServiceResult
{
    public T? Payload { get; }

    protected ServiceResult(bool isSucceeded, IReadOnlyList<Error>? errors) : base(isSucceeded, errors)
    {
        Payload = default;
    }
    
    protected ServiceResult(bool isSucceeded, IReadOnlyList<Error>? errors, T? payload) : base(isSucceeded, errors)
    {
        Payload = payload;
    }

    public new static ServiceResult<T> Success()
    {
        return new ServiceResult<T>(true, null);
    }

    public static ServiceResult<T> Success(T? data)
    {
        if(data is null)
            throw new ArgumentNullException(nameof(data),"Payload cannot be null for a Success result with payload");
        
        return new ServiceResult<T>(true, null, data);
    }

    public new static ServiceResult<T> Failure(params Error[] errors)
    {
        if(errors.Length == 0)
            throw new ArgumentException("At least one error must be provided", nameof(errors));
        
        return new ServiceResult<T>(false, errors.AsReadOnly());
    }
    
}