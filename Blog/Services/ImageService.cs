using Blog.Services.Interfaces;

namespace Blog.Services
{
    public class ImageService : IImageService
    {
        private readonly string _defaultProfileImageSrc = "/img/DefaultProfileImage.png";
        private readonly string _defaultBlogImageSrc = "/img/DefaultBlogImage.png";
        private readonly string _defaultCategoryImageSrc = "/img/DefaultCategoryImage.png";
        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType)
        {

            if (fileData == null || fileData.Length == 0)
            {
                switch (imageType)
                {
                    case 1: return _defaultProfileImageSrc;
                    case 2: return _defaultBlogImageSrc;
                    case 3: return _defaultCategoryImageSrc;
                }
            }
            try
            {
                string ImageBase64Data = Convert.ToBase64String(fileData!);
                return string.Format($"data:{extension};base64, {ImageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
