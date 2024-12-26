using Domain.Models.Dtos;
using Domain.Models.Entities;
using Domain.Models.Enums;

namespace Application.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupDto>> GetGroupDtosByUser(AppUser user);
        Task<AccessRight?> GetUserGroupAccessRights(AppUser user, int groupId);
        Task<GroupDto> GetGroupDtoById(int id);
        Task<SaveGroupDto> GetSaveGroupDtoById(int id);
        Task CreateNewGroup(SaveGroupDto saveGroupDto);
    }
}