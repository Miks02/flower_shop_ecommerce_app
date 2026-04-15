using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Infrastructure.InfrastructureErrors;
using FlowerShop.SharedKernel.ErrorCatalogue;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Infrastructure.Storage;

public class LocalFileStorage(ILogger<LocalFileStorage> logger, IConfiguration config) : IFileService
{
     public async Task<Result<string>> UploadFile(IFormFile file, string? uploadedFilePath, string? uploadSubDir)
    { 
        var fileValidationResult = IsFileValid(file);

        if (!fileValidationResult.IsSuccess)
        {
            logger.LogWarning("{error}", fileValidationResult.Errors[0].Description);
            return Result<string>.Failure(FileError.ValidationFailed());
        }

        var absoluteUploadDir = config["images"]!;
        var uploadDir = "uploads";

        if (!string.IsNullOrEmpty(uploadSubDir))
        {
            uploadSubDir = uploadSubDir.TrimStart('/');
            uploadDir = Path.Combine(uploadDir, uploadSubDir).Replace('\\', '/') + '/';
        }

        var uploadsDirPath = Path.Combine(absoluteUploadDir, uploadDir.TrimStart());

        if (!Directory.Exists(uploadsDirPath))
            Directory.CreateDirectory(uploadsDirPath);

        var sanitizedFileName = Path.GetFileName(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
        var filePath = Path.Combine(uploadsDirPath, uniqueFileName);

        if (!string.IsNullOrEmpty(uploadedFilePath))
            await DeleteFile(uploadedFilePath);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        return Result<string>.Success(uploadDir + uniqueFileName);

    }

    public Task<Result> DeleteFile(string filePath)
    {
        var oldFilePath = Path.Combine(config["images"]!,
            filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(oldFilePath))
        {
            logger.LogWarning("File does not exist");
            Result.Failure(GeneralError.NotFound(filePath));
        }

        try
        {
             File.Delete(oldFilePath);
        }
        catch (IOException ex)
        {
            logger.LogError("Unexpected error happened while trying to delete the file. {ex}", ex);
            throw;
        }

        return Task.FromResult(Result.Success());
    }

    private Result IsFileValid(IFormFile file)
    {
        var fileSize = file.Length;

        if (fileSize == 0)
            return Result.Failure(FileError.Empty());

        var maxFileSize = 5 * 1024 * 1024;

        if (fileSize > maxFileSize)
            return Result.Failure(FileError.TooLarge(maxFileSize));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return Result.Failure(FileError.UnsupportedExtension(extension));
        
        return Result.Success();

    }    
}