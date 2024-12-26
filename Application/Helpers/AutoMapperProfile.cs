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
                opt => opt.MapFrom(src => src.LecturerGroupRelations.Select(x => x.LecturerId)))
                .ReverseMap();
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.SubjectName,
                opt => opt.MapFrom(src => src.Subject.Name))
                .ForMember(dest => dest.Lecturers,
                opt => opt.MapFrom(src => src.LecturerGroupRelations.Select(lgr => lgr.Lecturer.User.UserName)));
            CreateMap<Meeting, MeetingDto>()
                .ForMember(dest => dest.GroupName,
                opt => opt.MapFrom(src => src.Group.Name));
        }
    }
}
