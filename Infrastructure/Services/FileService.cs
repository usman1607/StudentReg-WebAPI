using Application.Services.Contracts;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {

        private readonly Cloudinary _cloudClient;

        public FileService()
        {
            var account = new Account(
                "Root",
                "",
                ""
            );
            _cloudClient = new Cloudinary(account);
        }

        public async Task<string> UploadFileWithCloudinary(IFormFile file, string fileName)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new CloudinaryDotNet.FileDescription(fileName, stream)
            };

            var uploadResult = await _cloudClient.UploadAsync(uploadParams);
            return uploadResult.SecureUri.ToString();
        }
    }
}
