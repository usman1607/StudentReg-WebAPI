using Domain.Enums;

namespace Application.Services.Contracts
{
    public interface IFileServiceFactory
    {
        IFileService Create(FileServiceType type);
    }
}
