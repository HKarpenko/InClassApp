using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Domain.Models.Enums;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserService _userService;
        private readonly IMapper _autoMapper;
        private readonly ILecturersRepository _lecturersRepository;
        private readonly IStudentRepository _studentRepository;

        public GroupService(IGroupRepository groupRepository, IUserService userService, IMapper autoMapper,
            ILecturersRepository lecturersRepository, IStudentRepository studentRepository)
        {
            _groupRepository = groupRepository;
            _userService = userService;
            _autoMapper = autoMapper;
            _lecturersRepository = lecturersRepository;
            _studentRepository = studentRepository;
        }

        public async Task<List<GroupDto>> GetGroupDtosByUser(AppUser user)
        {
            var groups = await _groupRepository.GetAllAsNoTracking();
            var mainRole = await _userService.GetUserMainRole(user);

            if (mainRole == UserRole.Lecturer)
            {
                Lecturer currentLecturer = await _lecturersRepository.GetLecturerByUserIdAsNoTracking(user.Id) ??
                    throw new NullReferenceException($"Current user: {user.UserName} has role of Lecturer, but such entity not found");
                groups = groups.Where(x => x.LecturerGroupRelations.Any(r => r.LecturerId == currentLecturer.Id));
            }
            else if (mainRole == UserRole.Student)
            {
                var currentStudent = await _studentRepository.GetStudentByUserIdAsNoTracking(user.Id) ??
                    throw new NullReferenceException($"Current user: {user.UserName} has role of Student, but such entity not found");
                groups = groups.Where(x => x.StudentGroupRelations.Any(r => r.StudentId == currentStudent.Id));
            }

            return _autoMapper.Map<List<GroupDto>>(groups.ToList());
        }

        public async Task<AccessRight?> GetUserGroupAccessRights(AppUser user, int groupId)
        {
            var mainRole = await _userService.GetUserMainRole(user);
            if (mainRole == UserRole.Admin)
            {
                return AccessRight.ReadWrite;
            }
            else if (mainRole == UserRole.Lecturer)
            {
                var currentLecturer = await _lecturersRepository.GetLecturerByUserIdAsNoTracking(user.Id) ??
                    throw new NullReferenceException($"Current user: {user.UserName} has role of Lecturer, but such entity not found");
                return currentLecturer.LecturerGroupRelations.Any(r => r.GroupId == groupId) ? AccessRight.ReadWrite
                    : null;
            }
            else if (mainRole == UserRole.Student)
            {
                var currentStudent = await _studentRepository.GetStudentByUserIdAsNoTracking(user.Id) ??
                    throw new NullReferenceException($"Current user: {user.UserName} has role of Student, but such entity not found");

                return currentStudent.StudentGroupRelations.Any(r => r.GroupId == groupId) ? AccessRight.ReadOnly
                    : null;
            }
            
            return null;
        }

        public async Task<GroupDto> GetGroupDtoById(int id)
        {
            return _autoMapper.Map<GroupDto>(await _groupRepository.GetById(id));
        }

        public async Task<SaveGroupDto> GetSaveGroupDtoById(int id)
        {
            var group = await _groupRepository.GetById(id);
            return _autoMapper.Map<SaveGroupDto>(group);
        }

        public async Task CreateNewGroup(SaveGroupDto saveGroupDto)
        {
            Group newGroup = _autoMapper.Map<Group>(saveGroupDto);
            await _groupRepository.Add(newGroup);

            foreach (var lecturerId in saveGroupDto.LecturersIds)
            {
                await _groupRepository.AddLecturerGroupRelation(lecturerId, newGroup.Id);
            }
        }
    }
}
