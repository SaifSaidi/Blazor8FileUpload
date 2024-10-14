using Blazor8FileUpload.Services.FileManageServices.Enums;
using Blazor8FileUpload.Services.FileManageServices.ValueObjects;
using Microsoft.AspNetCore.Components.Forms; 

namespace Blazor8FileUpload.Services.FileManageServices;

public class FileUploadManager(IWebHostEnvironment webHostEnv)
{
    private string _uploadedImageUrl = "";
    public string FileUploadedPath => _uploadedImageUrl;

    public bool RemoveFile(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(webHostEnv.WebRootPath, filePath.TrimStart('/'));
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


    //public async Task<string> HandleFileUpload(FileModelValueObject fileModel, FileUploadCategoryEnum fileUploadCategoryEnum, HashSet<string> extensions, string? subDir = null)
    //{
    //    return await UploadFile(fileModel.File, fileModel.Name, fileUploadCategoryEnum, extensions, subDir, fileModel.Size);
    //}

    public async Task<bool> HandleBrowserFileUpload(SSRFileModelValueObject fileModel)
    {
        var file = fileModel.File ?? throw new Exception("Please upload file!");
        var sanitizeFileName = FileUploadManagerHelper.SanitizeFileName(fileModel.File.Name);
        var fileSize = fileModel.File.Size;
        return await UploadFile(file, sanitizeFileName, fileSize, fileModel.FileUploadCategory, fileModel.Extensions, fileModel.SubDir, fileModel.MaxSizeInMB);
    }

    private async Task<bool> UploadFile(object file, string? fileName, long fileSize, FileUploadCategoryEnum fileUploadTypeEnum, HashSet<string> extensions, string? subDir, double? maxSize)
    { 
        if (string.IsNullOrEmpty(fileName))
        {
            throw new Exception("Please upload correct file!");
        }

        string extension = FileUploadManagerHelper.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension) || !extensions.Contains(extension))
        {
            throw new Exception($"Only {string.Join(", ", extensions)} files are allowed, ({fileName}) file is {extension}.");
        }

        var sizeInMB = FileUploadManagerHelper.SizeInMB(fileSize);
     
        ValidateByFileCategory(fileUploadTypeEnum, sizeInMB, maxSize);
         
        var folderPathType = FileUploadManagerHelper.GetFolderPath(fileUploadTypeEnum, subDir);

        var finalFileName = $"{fileName.Substring(0, fileName.LastIndexOf('.')) ?? Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
        var folderPath = Path.Combine(webHostEnv.WebRootPath, "uploads", folderPathType);

        // if not exists, create one
        FileUploadManagerHelper.EnsureDirectoryExists(folderPath);
        var filePath = Path.Combine(folderPath, finalFileName);

        if (file is IFormFile formFileStream)
        { 
            await UploadFileStream(formFileStream.OpenReadStream(), filePath);
        }
        else if (file is IBrowserFile browserFileStream)
        {
            await UploadFileStream(browserFileStream.OpenReadStream(browserFileStream.Size), filePath);
        } else
        {
            return false;
        }

        _uploadedImageUrl = $"/uploads/{folderPathType}/{finalFileName}";
        return true;
    }

    private static async Task UploadFileStream(Stream fileStream, string filePath)
    {
        await using FileStream outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream, new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);
    }

    private static bool ValidateByFileCategory(FileUploadCategoryEnum fileUploadTypeEnum,  double sizeInMB, double? maxSizeInMb)
    {
        // Change as you need
        return fileUploadTypeEnum switch
        {
            FileUploadCategoryEnum.Images when sizeInMB > (maxSizeInMb ?? 0.5) =>
                throw new Exception($"Please upload a image, no larger than {maxSizeInMb ?? 0.5}MB."),
            FileUploadCategoryEnum.Docs when sizeInMB > (maxSizeInMb ?? 0.2) =>
                throw new Exception("Please upload a document, no larger than 0.2MB."),
            FileUploadCategoryEnum.Videos when sizeInMB > (maxSizeInMb ?? 2) =>
                throw new Exception("Please upload a videos, no larger than 2MB."),            
            _ => true,
        };
    }



}
