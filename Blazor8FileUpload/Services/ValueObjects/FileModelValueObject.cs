namespace Blazor8FileUpload.Services.ValueObjects
{
    public class FileModelValueObject
    {
        public string Name { get; set; } = "";
        public long Size { get; set; }
        public object File { get; set; }

        public override string ToString()
        {
            return $"File Name: {Name}, Size: {Size} ";
        }
    }
}
