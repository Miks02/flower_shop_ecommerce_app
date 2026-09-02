namespace FlowerShop.Application.Features.Auth.Commands.Login;

public record LoginCommand
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}