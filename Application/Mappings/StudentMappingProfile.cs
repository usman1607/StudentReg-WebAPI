using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<Student, StudentDto>();

            CreateMap<StudentRequestDto, Student>()
                .ForMember(dest => dest.MatricNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.HashSalt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
        }
    }
}
