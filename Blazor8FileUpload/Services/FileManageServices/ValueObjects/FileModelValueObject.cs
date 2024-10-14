using Blazor8FileUpload.Services.FileManageServices.Enums;
using System.Drawing;
using System.Xml.Linq;

namespace Blazor8FileUpload.Services.FileManageServices.ValueObjects
{
    public class FileModelValueObject
    { 
        public double? MaxSizeInMB { get; set; }
        public HashSet<string> Extensions;
        public string? SubDir { get; set; }
        public FileUploadCategoryEnum FileUploadCategory { get; set; }
      
    }
}
