using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Infrastructure.InfrastructureErrors;

public class FileError
{
    public static Error StorageFailed(string message = "Failed to store the uploaded file")
        => new("File.StorageFailed", message);

    public static Error Empty(string message = "Uploaded file is empty")
        => new("File.Empty", message);

    public static Error TooLarge(long maxBytes)
    {
        string message = $"Uploaded file exceeds maximum allowed size of {maxBytes} bytes";
        return new Error("File.TooLarge", message);
    }

    public static Error UnsupportedExtension(string extension = "")
    {
        string message = string.IsNullOrWhiteSpace(extension)
            ? "Uploaded file extension is not supported"
            : $"Uploaded file extension '{extension}' is not supported";
        return new Error("File.UnsupportedExtension", message);
    }

    public static Error ValidationFailed(string message = "Uploaded file failed validation")
        => new("File.ValidationFailed", message);
}