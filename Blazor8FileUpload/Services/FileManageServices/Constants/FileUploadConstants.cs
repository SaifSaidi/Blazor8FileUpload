namespace Blazor8FileUpload.Services.FileManageServices.Constants
{
    public class FileUploadConstants
    {
        public static readonly HashSet<string> ImageExtensions
         = new([".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"],
             StringComparer.OrdinalIgnoreCase);


        public static readonly HashSet<string> SafeExtensions
         = new([".doc", ".docx", ".pdf", ".txt", ".xls", ".xlsx", ".ppt", ".pptx", ".csv", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp3", ".mp4", ".zip", ".rar"], StringComparer.OrdinalIgnoreCase);


        public static HashSet<string> Extensions(string[] extensions)
        {
            return new HashSet<string>(extensions);
        }
    }
}
