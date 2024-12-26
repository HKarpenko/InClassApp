using Domain.Models.Dtos;

namespace Application.Interfaces
{
    public interface IMeetingService
    {
        Task<MeetingDto> GetMeetingDtosByGroupId(int groupId);
    }
}