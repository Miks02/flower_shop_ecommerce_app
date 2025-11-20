using System.Net.Security;

namespace FlowerShop.Services.Results;

public class OperationResult
{
    public List<string> Errors { get; set; } = new List<string>();

    public bool Success => Errors.Count == 0;
}