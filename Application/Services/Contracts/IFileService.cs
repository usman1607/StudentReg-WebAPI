using Microsoft.AspNetCore.Http;

namespace Application.Services.Contracts
{
    public interface IFileService
    {

        Task<string> UploadFile(IFormFile file, string fileName);
        Task<bool> DeleteFile(string fileName);
    }
}
