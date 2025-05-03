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
        return (await GetPresenceRecordDto(meetingId, currentStudent.Id)).Status;
    }

    public async Task<PresenceRecordDto> GetPresenceRecordDto(int meetingId, int studentId)
    {
        var presenceRecord = await presenceRecordsRepository.GetPresenceRecordsByMeetingId(meetingId).FirstOrDefaultAsync(x => x.StudentId == studentId);
        return mapper.Map<PresenceRecordDto>(presenceRecord);
    }

    public async Task CreatePresenceRecordForCurrentStudent(int meetingId)
    {
        var currentStudentId = (await studentService.GetStudentDtoOfCurrentUser()).Id;
        var presenceRecord = await GetPresenceRecordDto(meetingId, currentStudentId);
        if (presenceRecord == null)
        {
            await presenceRecordsRepository.Add(new PresenceRecord { MeetingId = meetingId, StudentId = currentStudentId });
        }        
    }

    public async Task<List<PresenceRecordDto>> GetPresenceRecordDtosByMeetingId(int meetingId)
    {
        var presenceRecords = await presenceRecordsRepository.GetPresenceRecordsByMeetingId(meetingId).ToListAsync();
        return mapper.Map<List<PresenceRecordDto>>(presenceRecords);
    }
}
