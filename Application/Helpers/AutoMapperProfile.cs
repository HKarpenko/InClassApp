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
            CreateMap<PresenceRecord, PresenceRecordDto>();
            CreateMap<Subject, SubjectDto>();
            CreateMap<Group, SaveGroupDto>()
                .ForMember(dest => dest.LecturersIds,
                    opt => opt.MapFrom(src => src.LecturerGroupRelations.Select(x => x.LecturerId)));
            CreateMap<SaveGroupDto, Group>()
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore());
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
                .ForMember(dest => dest.Lecturers,
                    opt => opt.MapFrom(src => src.LecturerGroupRelations.Select(lgr => $"{lgr.Lecturer.User.FirstName} {lgr.Lecturer.User.LastName}")));
            CreateMap<Meeting, MeetingDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));
            CreateMap<MeetingDto, Meeting>();
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
            CreateMap<Lecturer, LecturerDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
            CreateMap<UserRegistrationDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.FirstName}.{src.LastName}"));
        }
    }
}
