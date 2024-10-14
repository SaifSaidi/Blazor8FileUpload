using Microsoft.AspNetCore.Components.Forms;

namespace Blazor8FileUpload.Services.FileManageServices.ValueObjects
{
    public class SSRFileModelValueObject : FileModelValueObject
    {
        public IBrowserFile? File { get; set; }
    }
}
