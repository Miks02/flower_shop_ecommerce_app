namespace FlowerShop.Infrastructure.Htmx;

public enum ToastLevel
{
    Success,
    Error,
    Warning,
    Info
}

public class ToastMessage
{
    public string Message { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Level { get; set; } = "info";
    public int Duration { get; set; } = 4000;
}
