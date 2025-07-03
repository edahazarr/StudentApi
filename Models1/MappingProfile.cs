using AutoMapper;

namespace StudentApi.Models1
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            CreateMap<StudentDto, Student>();
        }
    }
}
