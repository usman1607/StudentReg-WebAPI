using Microsoft.AspNetCore.Http;

namespace Application.Services.Contracts
{
    public interface IFileService
    {

        Task<string> UploadFileWithCloudinary(IFormFile file, string fileName);
    }
}
