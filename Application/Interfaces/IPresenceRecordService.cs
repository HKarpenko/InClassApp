using Domain.Models.Dtos;

namespace Application.Interfaces;

public interface IPresenceRecordService
{
    Task<bool> GetCurrentStudentStatusByMeetingId(int meetingId);
    Task<PresenceRecordDto> GetPresenceRecordDto(int meetingId, int studentId);
    Task<List<PresenceRecordDto>> GetPresenceRecordDtosByMeetingId(int meetingId);
    Task CreatePresenceRecordForCurrentStudent(int meetingId);
}
