namespace FlowerShop.Services.Results;

public class ProfileUpdateResult : OperationResult
{
    public bool ProfileUpdated { get; set; }
    
    public bool PasswordChanged { get; set; }
}