using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PresenceRecordService(
    IMapper mapper,
    IPresenceRecordRepository presenceRecordsRepository,
    IStudentService studentService): IPresenceRecordService
{
    public async Task<bool> GetCurrentStudentStatusByMeetingId(int meetingId)
    {
        var currentStudent = await studentService.GetStudentDtoOfCurrentUser();
        return (await GetPresenceRecordDto(meetingId, currentStudent.Id))?.Status == true;
    }

    public async Task<PresenceRecordDto> GetPresenceRecordDto(int meetingId, int studentId)
    {
        var presenceRecord = await presenceRecordsRepository.GetPresenceRecordsByMeetingId(meetingId).FirstOrDefaultAsync(x => x.StudentId == studentId);
        return mapper.Map<PresenceRecordDto>(presenceRecord);
    }

    public async Task CheckInCurrentStudentByMeeting(int meetingId)
    {
        var currentStudentId = (await studentService.GetStudentDtoOfCurrentUser())?.Id ?? throw new DbUpdateException("Current user is not a student!");
        PresenceRecord presenceRecord = await presenceRecordsRepository.GetPresenceRecordsByMeetingId(meetingId)
            .FirstAsync(pr => pr.StudentId == currentStudentId);
        presenceRecord.Status = true;
        await presenceRecordsRepository.Update(presenceRecord);
    }

    public async Task<List<PresenceRecordDto>> GetPresenceRecordDtosByMeetingId(int meetingId)
    {
        var presenceRecords = await presenceRecordsRepository.GetPresenceRecordsByMeetingId(meetingId).ToListAsync();
        return mapper.Map<List<PresenceRecordDto>>(presenceRecords);
    }
}
