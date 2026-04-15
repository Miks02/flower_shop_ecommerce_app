using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Http;

namespace FlowerShop.Application.Common.Abstractions;

public interface IFileService
{
    Task<Result<string>> UploadFile(IFormFile file, string? uploadedFilePath, string? uploadSubDir);
    public Task<Result> DeleteFile(string filePath);
}