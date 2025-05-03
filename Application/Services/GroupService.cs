using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Domain.Models.Enums;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class GroupService(IGroupRepository groupRepository,
        IUserService userService,
        IMapper mapper,
        ILecturersRepository lecturersRepository,
        IStudentRepository studentRepository) : IGroupService
{
    public async Task<List<GroupDto>> GetGroupDtosByUser(AppUser user)
    {
        var groups = await groupRepository.GetAllAsNoTracking().ToListAsync();
        var mainRole = await userService.GetUserMainRole(user);

        if (mainRole == UserRole.Lecturer)
        {
            Lecturer currentLecturer = await lecturersRepository.GetLecturerByUserIdAsNoTracking(user.Id) ??
                throw new NullReferenceException($"Current user: {user.UserName} has role of Lecturer, but such entity not found");
            groups = groups.Where(x => x.LecturerGroupRelations.Any(r => r.LecturerId == currentLecturer.Id)).ToList();
        }
        else if (mainRole == UserRole.Student)
        {
            var currentStudent = await studentRepository.GetStudentByUserIdAsNoTracking(user.Id) ??
                throw new NullReferenceException($"Current user: {user.UserName} has role of Student, but such entity not found");
            groups = groups.Where(x => x.StudentGroupRelations.Any(r => r.StudentId == currentStudent.Id)).ToList();
        }

        return mapper.Map<List<GroupDto>>(groups);
    }

    public async Task<AccessRight?> GetUserGroupAccessRights(AppUser user, int groupId)
    {
        var mainRole = await userService.GetUserMainRole(user);
        if (mainRole == UserRole.Admin)
        {
            return AccessRight.ReadWrite;
        }
        else if (mainRole == UserRole.Lecturer)
        {
            var currentLecturer = await lecturersRepository.GetLecturerByUserIdAsNoTracking(user.Id) ??
                throw new NullReferenceException($"Current user: {user.UserName} has role of Lecturer, but such entity not found");
            return currentLecturer.LecturerGroupRelations.Any(r => r.GroupId == groupId) ? AccessRight.ReadWrite
                : null;
        }
        else if (mainRole == UserRole.Student)
        {
            var currentStudent = await studentRepository.GetStudentByUserIdAsNoTracking(user.Id) ??
                throw new NullReferenceException($"Current user: {user.UserName} has role of Student, but such entity not found");

            return currentStudent.StudentGroupRelations.Any(r => r.GroupId == groupId) ? AccessRight.ReadOnly
                : null;
        }

        return null;
    }

    public async Task<IEnumerable<GroupDto>> GetAllGroups()
    {
        var groups = await groupRepository.GetAllAsNoTracking().ToListAsync();
        return mapper.Map<List<GroupDto>>(groups);
    }

    public async Task<GroupDto> GetGroupDtoById(int id)
    {
        return mapper.Map<GroupDto>(await groupRepository.GetById(id));
    }

    public async Task<SaveGroupDto> GetSaveGroupDtoById(int id)
    {
        var group = await groupRepository.GetByIdAsNoTracking(id);
        return mapper.Map<SaveGroupDto>(group);
    }

    public async Task CreateNewGroup(SaveGroupDto saveGroupDto)
    {
        Group newGroup = mapper.Map<Group>(saveGroupDto);
        newGroup.SubjectId = saveGroupDto.SubjectId;
        await groupRepository.Add(newGroup);

        foreach (var lecturerId in saveGroupDto.LecturersIds)
        {
            await groupRepository.AddLecturerGroupRelation(lecturerId, newGroup.Id);
        }
    }

    public async Task UpdateGroup(AppUser user, SaveGroupDto saveGroupDto)
    {
        var currentUserRole = await userService.GetUserMainRole(user);

        var existingGroup = await groupRepository.GetByIdAsNoTracking(saveGroupDto.Id);
        mapper.Map(saveGroupDto, existingGroup);
        if (currentUserRole == UserRole.Admin)
        {
            var currentLecturerIds = existingGroup.LecturerGroupRelations.Select(x => x.LecturerId).ToList();
            await DeltaGroupLecturerRelationsNotSaved(saveGroupDto.Id, currentLecturerIds, saveGroupDto.LecturersIds);
            existingGroup.SubjectId = saveGroupDto.SubjectId;
        }
        await groupRepository.Update(existingGroup);
    }

    public async Task DeleteGroup(int groupId)
    {
        await groupRepository.Delete(groupId);
    }

    private async Task<bool> DeltaGroupLecturerRelationsNotSaved(int groupId, ICollection<int> currentLecturerIds, ICollection<int> newLecturerIds)
    {
        foreach (var lecturerId in currentLecturerIds.Except(newLecturerIds))
        {
            await groupRepository.DeleteLecturerGroupRelationNotSaved(lecturerId, groupId);
        }
        foreach (var lecturerId in newLecturerIds.Except(currentLecturerIds))
        {
            await groupRepository.AddLecturerGroupRelationNotSaved(lecturerId, groupId);
        }
        return true;
    }

    public async Task AddStudentGroupRelation(int studentId, int groupId)
    {
        await groupRepository.AddStudentGroupRelation(studentId, groupId);
    }

    public async Task DeleteStudentGroupRelation(int studentId, int groupId)
    {
        await groupRepository.DeleteStudentGroupRelation(studentId, groupId);
    }
}
