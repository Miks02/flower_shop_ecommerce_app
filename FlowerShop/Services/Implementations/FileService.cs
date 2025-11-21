using FlowerShop.Services.Interfaces;

namespace FlowerShop.Services.Implementations;

public class FileService : BaseService<FileService>, IFileService
{

    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment, IHttpContextAccessor http, ILogger<FileService> logger) : base(http, logger)
    {
        _environment = environment;
    }
    
    
    public async Task<string> UploadFile(IFormFile file, string? uploadedFilePath, string uploadSubDir = "")
    {
        string uploadDir = "/Uploads/";

        if (!string.IsNullOrWhiteSpace(uploadSubDir))
        {
            uploadSubDir = uploadSubDir.TrimStart('/');
            uploadDir = Path.Combine(uploadDir, uploadSubDir).Replace('\\', '/') + '/';
        }
        
        var uploadsDirPath = Path.Combine(_environment.WebRootPath, uploadDir.TrimStart('/'));

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
        var oldFilePath = Path.Combine(_environment.WebRootPath, 
            filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (!System.IO.File.Exists(oldFilePath))
        {
            LogInfo("No file to delete");
            return true;
        }

        try
        {
            System.IO.File.Delete(oldFilePath);
            return true;
        }
        catch (IOException ex)
        {
            LogError("Unexpected error happened while trying to delete the file. " + ex);
            return false;
        }
        
    }
}