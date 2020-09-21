
namespace Foundry.Domain.ApiModel
{
   public class FileuploadModel
    {
        public string ImageUrl { get; set; }
        public string Type { get; set; }
    }

    public class FileuploadModelS3
    {
        public byte[] ImageByte { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
    }
}
