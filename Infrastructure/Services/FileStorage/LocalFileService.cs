using Application.Configurations;
using Application.Exceptions;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.FileStorage
{
    public class LocalFileService : IFileService
    {
        private readonly string _basePath;
        private readonly ILogger<LocalFileService> _logger;

        public LocalFileService(IOptions<StorageSettings> config, ILogger<LocalFileService> logger)
        {
            var settings = config.Value;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), settings.BasePath);

            // Ensure directory exists
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            _logger = logger;
        }

        public async Task<string> UploadFile(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");
            
            var filePath = Path.Combine(_basePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            return filePath;
        }

        public Task<bool> DeleteFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileName}", fileName);
                throw new ValidationException("Failed to delete file {publicId}");
            }
        }
    }
}
