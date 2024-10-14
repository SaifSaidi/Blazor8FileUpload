using Blazor8FileUpload.Services.Enums;
using Blazor8FileUpload.Services.ValueObjects;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;

namespace Blazor8FileUpload.Services;

public class FileUploadManager
{
    private readonly IWebHostEnvironment _webHostEnv;
    private string _uploadedImageUrl = "";
    public string FileUploadedPath => _uploadedImageUrl;
    public FileUploadManager(IWebHostEnvironment webHostEnv)
    {
        _webHostEnv = webHostEnv;
    }

    public async Task<string> HandleFileUpload(FileModelValueObject fileModel, FileUploadCategoryEnum fileUploadCategoryEnum, HashSet<string> extensions, string? subDir = null)
    {
        return await UploadFile(fileModel.File, fileModel.Name, fileUploadCategoryEnum, extensions, subDir, fileModel.Size);
    }

    public async Task<string> HandleBrowserFileUpload(FileModelValueObject fileModel, FileUploadCategoryEnum fileUploadTypeEnum, HashSet<string> extensions, string? subDir = null)
    {
        return await UploadFile(fileModel.File, fileModel.Name, fileUploadTypeEnum, extensions, subDir, fileModel.Size);
    }

    private async Task<string> UploadFile(object file, string? fileName, FileUploadCategoryEnum fileUploadTypeEnum, HashSet<string> extensions, string? subDir, long fileSize)
    {
        if (file == null)
        {
            throw new Exception("Please upload file!");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new Exception("Please upload correct file!");
        }

        string extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension) || !extensions.Contains(extension))
        {
            throw new Exception($"Only {string.Join(", ", extensions)} files are allowed, {fileName} file is {extension}.");
        }

        var sizeInMB = SizeInMB(fileSize);
        var folderPathType = GetFolderPathType(fileUploadTypeEnum, subDir, sizeInMB);

        var finalFileName = $"{fileName.Substring(0, fileName.LastIndexOf('.')) ?? Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
        var folderPath = Path.Combine(_webHostEnv.WebRootPath, "uploads", folderPathType);

        EnsureDirectoryExists(folderPath);
        var filePath = Path.Combine(folderPath, finalFileName);

        try
        {

            if (file is IFormFile formFileStream)
            {

                await UploadFileStream(formFileStream.OpenReadStream(), filePath);

            }
            else if (file is IBrowserFile browserFileStream)
            {
                await UploadFileStream(browserFileStream.OpenReadStream(browserFileStream.Size), filePath);

            }

            _uploadedImageUrl = $"/uploads/{folderPathType}/{finalFileName}";
            return "Upload successful.";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception("Cannot upload.");
        }
    }

    public bool RemoveFile(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_webHostEnv.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            else
            {
                Console.WriteLine($"File not found: {fullPath}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting file: {ex.Message}");
            return false;
        }
    }

    private static async Task UploadFileStream(Stream fileStream, string filePath)
    {
        await using FileStream outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream, new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);
    }

    public string SanitizeFileName(string fileName)
    {
        // Remove invalid characters from the file name
        var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        var regex = new Regex($"[{Regex.Escape(invalidChars)}]");
        var filter = regex.Replace(fileName, "_").Replace(" ", "_");
        return filter;
    }
    public string GetExtension(string fileName)
    {

        return Path.GetExtension(fileName);
    }

    public double SizeInMB(long bytes) => Math.Round((bytes) / (1024.0 * 1024.0), 2);

    public bool FileIsExists(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_webHostEnv.WebRootPath, filePath.TrimStart('/'));
            return File.Exists(fullPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking file existence: {ex.Message}");
            return false;
        }
    }
    private static string GetFolderPathType(FileUploadCategoryEnum fileUploadTypeEnum, string? subDir, double sizeInMB)
    {
        string folderPathType = fileUploadTypeEnum.ToString();

        if (subDir != null)
        {
            folderPathType = $"{folderPathType}/{subDir}";
        }

        return fileUploadTypeEnum switch
        {
            FileUploadCategoryEnum.Category_SVG when sizeInMB > 0.2 =>
            throw new Exception("Please upload a category svg no larger than 0.2MB."),
            FileUploadCategoryEnum.blogs when sizeInMB > 1.0
            => throw new Exception("Blog image max size is 1MB"),
            _ => folderPathType,
        };
    }

    private static void EnsureDirectoryExists(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

}
