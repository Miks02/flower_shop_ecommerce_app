namespace FlowerShop.Web.ViewModels.Components;

public record RegisterViewModel
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Username { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string PhoneNumber { get; init; } = null!;

    public string Password { get; init; } = null!;

    public string ConfirmPassword { get; init; } = null!;
}