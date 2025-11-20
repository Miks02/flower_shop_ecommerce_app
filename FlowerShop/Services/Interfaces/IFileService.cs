namespace FlowerShop.Services.Interfaces;

public interface IFileService
{
    Task<string> UploadFile(IFormFile file, string? uploadedFilePath, string uploadSubDir = "");

    void DeleteFile(string filePath);
}