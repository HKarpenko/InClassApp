using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;

namespace Application.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Subject, SaveSubjectDto>()
                .ReverseMap();
            CreateMap<Group, SaveGroupDto>()
                .ForMember(dest => dest.LecturersIds,
                    opt => opt.MapFrom(src => src.LecturerGroupRelations.Select(x => x.LecturerId)));
            CreateMap<SaveGroupDto, Group>()
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore());
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
                .ForMember(dest => dest.Lecturers,
                    opt => opt.MapFrom(src => src.LecturerGroupRelations.Select(lgr => lgr.Lecturer.User.UserName)));
            CreateMap<Meeting, MeetingDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.FirstName));
            CreateMap<UserRegistrationDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        }
    }
}
