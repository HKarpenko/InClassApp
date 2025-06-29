using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Domain.Models.Dtos;
using Application.Interfaces;
using Domain.Models.Enums;

namespace InClassApp.Controllers;

/// <summary>
/// Controller for group management
/// </summary>
[Authorize]
public class GroupsController(
    ISubjectService _subjectService,
    IUserService _userService,
    IGroupService _groupService,
    ILecturerService _lecturerService,
    IMeetingService _meetingService,
    IStudentService _studentService,
    UserManager<AppUser> _userManager) : Controller
{
    /// <summary>
    /// Gets view with all groups for current user
    /// </summary>
    /// <returns>Groups list view</returns>
    [HttpGet]
    public async Task<IActionResult> GroupsList()
    {
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        var userMainRole = currentUser != null ? await _userService.GetUserMainRole(currentUser) : null;
        if (currentUser == null || userMainRole == null)
        {
            return Unauthorized();
        }
        ViewData["CurrentRole"] = userMainRole;
        var groups = await _groupService.GetGroupDtosByUser(currentUser);

        return View(groups);
    }

    /// <summary>
    /// Gets view with group details
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <returns>Group details view</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int groupId)
    {
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        var accessRight = currentUser != null ? await _groupService.GetUserGroupAccessRights(currentUser, groupId)
            : null;
        if (accessRight == null)
        {
            return Unauthorized();
        }
        ViewData["Rights"] = accessRight;

        var group = await _groupService.GetGroupDtoById(groupId);
        if (group == null)
        {
            return NotFound();
        }

        ViewData["Meetings"] = await _meetingService.GetMeetingDtosByGroupId(groupId);
        return View(group);
    }

    /// <summary>
    /// Gets view with group create form
    /// </summary>
    /// <returns>Create group view</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Subjects"] = await _subjectService.GetAllSubjectDtos();
        ViewData["Lecturers"] = await _lecturerService.GetAllLecturerDtos();
        return View();
    }

    /// <summary>
    /// Saves given group if it is valid
    /// </summary>
    /// <param name="groupDto">Group dto to save</param>
    /// <returns>Group list view if saved successfully; otherwise showes an error message</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name, StudiesSemestr, SubjectId, LecturersIds, StartDate, EndDate")] SaveGroupDto groupDto)
    {
        if (ModelState.IsValid)
        {
            await _groupService.CreateNewGroup(groupDto);
            return RedirectToAction(nameof(GroupsList));
        }

        ViewData["Subjects"] = await _subjectService.GetAllSubjectDtos();
        ViewData["Lecturers"] = await _lecturerService.GetAllLecturerDtos();
        return View();
    }

    /// <summary>
    /// Gets view with group edit form
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <returns>Edit group view</returns>
    [Authorize(Roles = "Admin, Lecturer")]
    [HttpGet]
    public async Task<IActionResult> Edit(int groupId)
    {
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        var accessRight = currentUser != null ? await _groupService.GetUserGroupAccessRights(currentUser, groupId)
            : null;
        if (accessRight != AccessRight.ReadWrite)
        {
            return Unauthorized();
        }

        var group = await _groupService.GetSaveGroupDtoById(groupId);
        if (group == null)
        {
            return NotFound();
        }

        var currentUserRole = await _userService.GetUserMainRole(currentUser!);

        ViewData["Role"] = currentUserRole;
        ViewData["Subjects"] = await _subjectService.GetAllSubjectDtos();
        ViewData["Lecturers"] = await _lecturerService.GetAllLecturerDtos();
        return View(group);
    }

    /// <summary>
    /// Saves edited group if form is valid
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <param name="groupDto">Group dto to save</param>
    /// <returns>Group list view if saved successfully; otherwise showes an error message</returns>
    [HttpPost]
    [Authorize(Roles = "Admin, Lecturer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int groupId, [Bind("Id, Name, StudiesSemestr, SubjectId, LecturersIds, StartDate, EndDate")] SaveGroupDto groupDto)
    {
        if (groupId != groupDto.Id)
        {
            return NotFound();
        }
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        var accessRight = currentUser != null ? await _groupService.GetUserGroupAccessRights(currentUser, groupId)
            : null;
        if (accessRight != AccessRight.ReadWrite)
        {
            return Unauthorized();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _groupService.UpdateGroup(currentUser!, groupDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await GroupExists(groupDto.Id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(GroupsList));
        }

        ViewData["Subjects"] = await _subjectService.GetAllSubjectDtos();
        ViewData["Lecturers"] = await _lecturerService.GetAllLecturerDtos();
        return View(groupDto);
    }

    /// <summary>
    /// Gets view with group delete panel
    /// </summary>
    /// <param name="id">Group id</param>
    /// <returns>Group delete panel view</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(int groupId)
    {
        var group = await _groupService.GetGroupDtoById(groupId);
        if (group == null)
        {
            return NotFound();
        }

        return View(group);
    }

    /// <summary>
    /// Deletes group by id
    /// </summary>
    /// <param name="id">Group id</param>
    /// <returns>Groups list view</returns>
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int groupId)
    {
        await _groupService.DeleteGroup(groupId);
        return RedirectToAction(nameof(GroupsList));
    }

    /// <summary>
    /// Gets view with group students list
    /// </summary>
    /// <param name="id">Group id</param>
    /// <returns>Group students list view</returns>
    [HttpGet]
    [Authorize(Roles = "Admin, Lecturer")]
    [Route("Groups/StudentsList/{groupId:int}")]
    public async Task<IActionResult> StudentsList(int groupId)
    {
        if (!(await VerifyGroupWritePrivilege(groupId)))
        {
            return Unauthorized();
        }

        ViewData["GroupId"] = groupId;
        var students = await _studentService.GetStudentDtosByGroupId(groupId);
        return View(students);
    }

    /// <summary>
    /// Gets view with adding student to list form
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <returns>Add student to group view</returns>
    [HttpGet]
    [Authorize(Roles = "Admin, Lecturer")]
    [Route("Groups/AddStudent/{groupId:int}")]
    public async Task<IActionResult> AddStudent(int groupId)
    {
        if (!(await VerifyGroupWritePrivilege(groupId)))
        {
            return Unauthorized();
        }

        var addedStudents = (await _studentService.GetStudentDtosByGroupId(groupId)).Select(s => s.Id).ToList();
        var students = await _studentService.GetAllStudentsExcept(addedStudents);
        ViewData["Indecies"] = new SelectList(students, "Index", "Index");
        ViewData["GroupId"] = groupId;

        return View();
    }

    /// <summary>
    /// Adds student by index to group
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <param name="index">Students index</param>
    /// <returns>Group students list view if saved successfully; otherwise shows an error message</returns>
    [HttpPost]
    [Route("Groups/AddStudent/{groupId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStudent(int groupId, [Bind("Index")] string index)
    {
        if (!(await VerifyGroupWritePrivilege(groupId)))
        {
            return Unauthorized();
        }

        if (ModelState.IsValid)
        {
            var student = await _studentService.GetStudentDtoByIndex(index);
            var group = await _groupService.GetGroupDtoById(groupId);
            await _groupService.AddStudentGroupRelation(student.Id, group.Id);

            return RedirectToAction("StudentsList", "Groups", new { id = groupId });
        }

        var students = await _groupService.GetAllGroups();
        ViewData["Indecies"] = new SelectList(students, "Index", "Index");
        ViewData["GroupId"] = groupId;

        return View();
    }

    /// <summary>
    /// Gets group student remove panel
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <param name="studentId">Student id</param>
    /// <returns>Group student panel view</returns>
    [HttpGet]
    [Authorize(Roles = "Admin, Lecturer")]
    public async Task<IActionResult> RemoveStudent(int groupId, int studentId)
    {
        if (!(await VerifyGroupWritePrivilege(groupId)))
        {
            return Unauthorized();
        }

        var student = await _studentService.GetStudentDtoById(studentId);
        if (student == null)
        {
            return NotFound();
        }

        ViewData["GroupId"] = groupId;
        return View(student);
    }

    /// <summary>
    /// Removes student form group
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <param name="studentId">Student id</param>
    /// <returns>Group students list view</returns>
    [HttpPost, ActionName("RemoveStudent")]
    [Authorize(Roles = "Admin, Lecturer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveStudentConfirmed(int groupId, int studentId)
    {
        if (!(await VerifyGroupWritePrivilege(groupId)))
        {
            return Unauthorized();
        }

        await _groupService.DeleteStudentGroupRelation(studentId, groupId);
        return RedirectToAction("StudentsList", "Groups", new { id = groupId });
    }

    private async Task<bool> GroupExists(int id)
    {
        var groups = await _groupService.GetAllGroups();
        return groups.Any(e => e.Id == id);
    }

    private async Task<bool> VerifyGroupWritePrivilege(int groupId)
    {
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        var accessRight = currentUser != null ? await _groupService.GetUserGroupAccessRights(currentUser, groupId)
            : null;
        return accessRight == AccessRight.ReadWrite;
    }
}
