using Domain.Models.Dtos;

namespace Application.Interfaces
{
    public interface IMeetingService
    {
        Task<MeetingDto> GetMeetingDtosByGroupId(int groupId);
        Task<MeetingDto> GetMeetingDtoById(int id);
        Task<string> GetMeetingDecryptedCode(int id);
        Task CreateNewMeeting(MeetingDto meetingDto);
        Task UpdateMeeting(MeetingDto meetingDto);
        Task DeleteMeeting(int id);
        Task<bool> ValidateCode(int meetingId, string providedCode);
        Task<bool> IsAttendanceCheckLaunched(int meetingId);
        Task SwitchAttendanceCheckStatus(int meetingId, bool checkValue);
    }
}