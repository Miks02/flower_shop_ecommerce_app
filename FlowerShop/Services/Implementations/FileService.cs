using FlowerShop.Web.Services.Interfaces;

namespace FlowerShop.Web.Services.Implementations;

public class FileService(IWebHostEnvironment environment, ILogger<FileService> logger) : IFileService
{
    public async Task<string> UploadFile(IFormFile file, string? uploadedFilePath, string uploadSubDir = "")
    {
        string uploadDir = "/Uploads/";

        if (!string.IsNullOrWhiteSpace(uploadSubDir))
        {
            uploadSubDir = uploadSubDir.TrimStart('/');
            uploadDir = Path.Combine(uploadDir, uploadSubDir).Replace('\\', '/') + '/';
        }
        
        var uploadsDirPath = Path.Combine(environment.WebRootPath, uploadDir.TrimStart('/'));

        if (!Directory.Exists(uploadsDirPath))
            Directory.CreateDirectory(uploadsDirPath);

        var sanitizedFileName = Path.GetFileName(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
        var filePath = Path.Combine(uploadsDirPath, uniqueFileName);

        if (!string.IsNullOrEmpty(uploadedFilePath))
            DeleteFile(uploadedFilePath);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        return uploadDir + uniqueFileName;
    }   

    public bool DeleteFile(string filePath)
    {
        var oldFilePath = Path.Combine(environment.WebRootPath, 
            filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(oldFilePath))
        {
            logger.LogInformation("No file to delete");
            return true;
        }

        try
        {
            File.Delete(oldFilePath);
            return true;
        }
        catch (IOException ex)
        {
            logger.LogError("Unexpected error happened while trying to delete the file. {ex}", ex);
            return false;
        }
        
    }
}