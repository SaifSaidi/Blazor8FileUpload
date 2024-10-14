using Blazor8FileUpload.Services.FileManageServices.Enums;
using System.Text.RegularExpressions;

namespace Blazor8FileUpload.Services.FileManageServices
{
    public static class FileUploadManagerHelper
    {

        

        public static string SanitizeFileName(string fileName)
        {
            // Remove invalid characters from the file name
            var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var regex = new Regex($"[{Regex.Escape(invalidChars)}]");
            var filter = regex.Replace(fileName, "_").Replace(" ", "_");
            return filter;
        }

        public static string GetExtension(string fileName)
        { 
            return Path.GetExtension(fileName);
        }

        public static double SizeInMB(long bytes) => Math.Round(bytes / (1024.0 * 1024.0), 2);

        public static bool FileIsExists(string root, string filePath)
        {
            try
            {
                var fullPath = Path.Combine(root, filePath.TrimStart('/'));
                return File.Exists(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking file existence: {ex.Message}");
                return false;
            }
        }

        public static string GetFolderPath(FileUploadCategoryEnum fileUploadTypeEnum, string? subDir)
        {
            string folderPath = fileUploadTypeEnum.ToString();

            if (subDir != null)
            {
                folderPath = $"{folderPath}/{subDir}";
            }
            return folderPath;

        }
      
        public static void EnsureDirectoryExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

    }
}
