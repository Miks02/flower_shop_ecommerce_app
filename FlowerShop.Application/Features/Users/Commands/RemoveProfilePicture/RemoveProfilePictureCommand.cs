namespace FlowerShop.Application.Features.Users.Commands.RemoveProfilePicture;

public record RemoveProfilePictureCommand
{
    public string UserId { get; set; } = null!;
};