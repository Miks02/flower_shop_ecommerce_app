namespace FlowerShop.Web.ViewModels.Components;

public record ProfileSettingsViewModel
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string FullNameInitials { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string PhoneNumber { get; init; } = null!;

    public IFormFile? ProfilePicture { get; init; }

    public string? ImagePath { get; init; }
}