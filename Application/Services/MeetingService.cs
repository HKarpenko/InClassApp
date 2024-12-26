using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly IMapper _autoMapper;
        private readonly IMeetingRepository _meetingRepository;

        public MeetingService(IMapper autoMapper, IMeetingRepository meetingRepository)
        {
            _autoMapper = autoMapper;
            _meetingRepository = meetingRepository;
        }

        public async Task<MeetingDto> GetMeetingDtosByGroupId(int groupId)
        {
            return _autoMapper.Map<MeetingDto>(await _meetingRepository.GetMeetingsByGroupId(groupId));
        }
    }
}
