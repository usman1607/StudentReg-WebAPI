using Application.Services.Contracts;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.FileStorage
{
    public class FileServiceFactory : IFileServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FileServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFileService Create(FileServiceType type)
        {
            return type switch
            {
                FileServiceType.Cloudinary => _serviceProvider.GetRequiredService<CloudinaryFileService>(),
                FileServiceType.Local => _serviceProvider.GetRequiredService<LocalFileService>(),
                FileServiceType.AWS_S3 => _serviceProvider.GetRequiredService<AWSFileService>(),
                _ => throw new ArgumentException("Invalid file service type")
            };
        }
    }
}
