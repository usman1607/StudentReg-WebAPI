using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterStudentAsync(StudentRequestDto request);
        string GetSignedInEmail();
        bool IsStudent();
    }
}
