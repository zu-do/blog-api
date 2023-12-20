using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;
using TheBlog_API.Services;

namespace TheBlog_Tests
{
    public class ImageServiceTests
    {
        [Fact]
        public async Task SaveImage_ShouldReturnValidImageName()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var contentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "ImagesTest");
            mockEnvironment.Setup(m => m.ContentRootPath).Returns(contentRootPath);

            var imageService = new ImageService(mockEnvironment.Object);

            var formFile = CreateFormFile("testImage.jpg");

            var result = await imageService.SaveImage(formFile);

            Assert.NotNull(result);
            Assert.True(result.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase));
            Assert.True(result.Length > 10);
        }

        [Fact]
        public void DeleteImage_ExistingImage_ShouldDeleteImage()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var contentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "ImagesTest");
            mockEnvironment.Setup(m => m.ContentRootPath).Returns(contentRootPath);

            var imageService = new ImageService(mockEnvironment.Object);

            var imageName = "testImageToDelete.jpg";
            var imagePath = Path.Combine(contentRootPath, "Images", imageName);

            Directory.CreateDirectory(Path.Combine(contentRootPath, "Images"));
            File.Create(imagePath).Close();

            imageService.DeleteImage(imageName);

            Assert.False(File.Exists(imagePath));
        }

        private IFormFile CreateFormFile(string fileName)
        {
            var content = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A };
            var ms = new MemoryStream(content);
            return new FormFile(ms, 0, content.Length, "testImage", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}
