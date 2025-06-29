using Application.Helpers;
using Application.Helpers.Interfaces;
using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Application.Services;

public class MeetingService(
    IMapper mapper, 
    IMeetingRepository meetingRepository,
    IAttendanceCodeManager attendanceCodeManager) : IMeetingService
{
    public async Task<List<MeetingDto>> GetMeetingDtosByGroupId(int groupId)
    {
        return mapper.Map<List<MeetingDto>>(await meetingRepository.GetMeetingsByGroupId(groupId));
    }

    public async Task<MeetingDto> GetMeetingDtoById(int id)
    {
        return mapper.Map<MeetingDto>(await meetingRepository.GetById(id));
    }

    public async Task<string> GetMeetingDecryptedCode(int id)
    {
        var meeting = await meetingRepository.GetByIdAsNoTracking(id);
        return attendanceCodeManager.GetDecryptedCode(meeting.LastlyGeneratedCheckCode, meeting.LastlyGeneratedCodeIV);
    }

    public async Task CreateNewMeeting(MeetingDto meetingDto)
    {
        var meeting = mapper.Map<Meeting>(meetingDto);
        await meetingRepository.Add(meeting);
    }

    public async Task UpdateMeeting(MeetingDto meetingDto)
    {
        var meeting = await meetingRepository.GetByIdAsNoTracking(meetingDto.Id) ?? throw new NullReferenceException("Updated meeting doesn't exist");
        meeting.MeetingStartDate = meetingDto.MeetingStartDate;
        meeting.MeetingEndDate = meetingDto.MeetingEndDate;
        await meetingRepository.Update(meeting);
    }

    public async Task DeleteMeeting(int id)
    {
        await meetingRepository.Delete(id);
    }

    public async Task<bool> ValidateCode(int meetingId, string providedCode)
    {
        var meeting = await meetingRepository.GetByIdAsNoTracking(meetingId);
        var decryptedCode = attendanceCodeManager.GetDecryptedCode(meeting.LastlyGeneratedCheckCode, meeting.LastlyGeneratedCodeIV);

        return decryptedCode.Equals(providedCode);
    }

    public async Task<bool> IsAttendanceCheckLaunched(int meetingId)
    {
        var meeting = await meetingRepository.GetByIdAsNoTracking(meetingId);
        return meeting.IsAttendanceCheckLaunched;
    }

    public async Task SwitchAttendanceCheckStatus(int meetingId, bool checkValue)
    {
        var meeting = await meetingRepository.GetByIdAsNoTracking(meetingId);
        if (checkValue)
        {
            meeting.IsAttendanceCheckLaunched = true;
            var generatedCode = attendanceCodeManager.CreateAttendanceCode();
            attendanceCodeManager.EncryptMeeting(meeting, generatedCode);

            await meetingRepository.Update(meeting);
        }
        else
        {
            meeting.IsAttendanceCheckLaunched = false;
            await meetingRepository.Update(meeting);
        }
    }
}