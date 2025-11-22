using System.Net.Security;

namespace FlowerShop.Services.Results;

public class OperationResult<T> where T : class
{
    public List<string> Errors { get; set; } = new List<string>();

    public bool Succeeded => Errors.Count == 0;
    
    public T? Payload { get; set; }
}