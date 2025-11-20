using FlowerShop.Services.Interfaces;

namespace FlowerShop.Services.Implementations;

public class FileService : IFileService
{

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileService> _logger;

    public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
    {
        _environment = environment;
        _logger = logger;
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

    public void DeleteFile(string filePath)
    {
        var oldFilePath = Path.Combine(_environment.WebRootPath, 
            filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        
        if(System.IO.File.Exists(oldFilePath))
            System.IO.File.Delete(oldFilePath);
        
    }
}