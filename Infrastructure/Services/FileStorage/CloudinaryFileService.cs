using Application.Configurations;
using Application.Services.Contracts;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.FileStorage
{
    public class CloudinaryFileService : IFileService
    {
        private readonly Cloudinary _cloudClient;
        private readonly ILogger<CloudinaryFileService> _logger;
        private readonly CloudinarySettings _cloudinaryConfig;

        public CloudinaryFileService(IOptions<CloudinarySettings> config, ILogger<CloudinaryFileService> logger)
        {
            _logger = logger;
            _cloudinaryConfig = config.Value;
            var account = $"cloudinary://{_cloudinaryConfig.ApiKey}:{_cloudinaryConfig.ApiSecret}@{_cloudinaryConfig.CloudName}";
            _cloudClient = new Cloudinary(account);
        }

        public Task<bool> DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public async Task<string> UploadFile(IFormFile file, string fileName)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream)
            };

            var uploadResult = await _cloudClient.UploadAsync(uploadParams);
            return uploadResult.SecureUri.ToString();
        }
    }
}
