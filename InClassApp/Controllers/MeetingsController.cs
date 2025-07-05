using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Domain.Models.Dtos;

namespace InClassApp.Controllers;

/// <summary>
/// Controller for meeting management
/// </summary>
[Authorize]
public class MeetingsController(
    IMeetingService meetingService,
    IPresenceRecordService presenceRecordService,
    IGroupService groupService) : Controller
{
    /// <summary>
    /// Gets view with meetings details for admin or lecturer
    /// </summary>
    /// <param name="meetingId">Meeting id</param>
    /// <returns>Meeting details view for admin or lecturer</returns>
    [Authorize(Roles = "Admin, Lecturer")]
    [HttpGet]
    [Route("Meetings/Details/{meetingId}")]
    public async Task<IActionResult> Details(int meetingId)
    {
        var meeting = await meetingService.GetMeetingDtoById(meetingId);
        if (meeting == null)
        {
            return NotFound();
        }

        ViewData["Records"] = await presenceRecordService.GetPresenceRecordDtosByMeetingId(meetingId);
        ViewData["DecryptedCode"] = await meetingService.GetMeetingDecryptedCode(meetingId);
        ViewData["IsCheckingLaunched"] = await meetingService.IsAttendanceCheckLaunched(meetingId);

        return View(meeting);
    }

    /// <summary>
    /// Gets view with meetings details for student
    /// </summary>
    /// <param name="id">Meeting id</param>
    /// <returns>Meeting details view for student</returns>
    [Authorize(Roles = "Admin, Student")]
    [HttpGet]
    public async Task<IActionResult> DetailsStudent(int meetingId)
    {
        var meeting = await meetingService.GetMeetingDtoById(meetingId);
        if (meeting == null)
        {
            return NotFound();
        }

        ViewData["Status"] = await presenceRecordService.GetCurrentStudentStatusByMeetingId(meetingId);
        ViewData["IsLaunched"] = await meetingService.IsAttendanceCheckLaunched(meetingId);
        return View(meeting);
    }

    /// <summary>
    /// Checks the code validity and sets the current student attendance status
    /// </summary>
    /// <param name="meetingId">Meeting id</param>
    /// <param name="providedCode">Code provided by current user</param>
    /// <returns><c>true</c> if code is correct; otherwise <c>false</c></returns>
    [HttpPost]
    [Authorize(Roles = "Admin, Student")]
    [Route("Meetings/ValidateCode")]
    public async Task<bool> CodeValidationRealesed(int meetingId, string providedCode)
    {
        bool isCodeValid = await meetingService.ValidateCode(meetingId, providedCode);
        if (isCodeValid)
        {
            await presenceRecordService.CheckInCurrentStudentByMeeting(meetingId);
        }
        return isCodeValid;
    }

    /// <summary>
    /// Gets view for meeting create form
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <returns>Meeting create form view</returns>
    [Authorize(Roles = "Admin, Lecturer")]
    [HttpGet("Create/{groupId}")]
    public IActionResult Create(int groupId)
    {
        ViewData["GroupId"] = groupId;
        return View();
    }

    /// <summary>
    /// Saves given meeting if it is valid
    /// </summary>
    /// <param name="groupId">Group id</param>
    /// <param name="meeting">Meeting to save</param>
    /// <returns>Meetings list view if saved successfully; otherwise showes an error message</returns>
    [HttpPost("Create/{groupId}")]
    [Authorize(Roles = "Admin, Lecturer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int groupId, [Bind("MeetingStartDate,MeetingEndDate,GroupId,Id")] MeetingDto meeting)
    {
        if (ModelState.IsValid)
        {
            await meetingService.CreateNewMeeting(meeting);
            return RedirectToAction("Details", "Groups", new { groupId });
        }

        ViewData["GroupId"] = groupId;
        return View(meeting);
    }

    /// <summary>
    /// Gets view with meeting edit form
    /// </summary>
    /// <param name="id">Meeting id</param>
    /// <returns>Edit meeting view</returns>
    [Authorize(Roles = "Admin, Lecturer")]
    [HttpGet]
    public async Task<IActionResult> Edit(int meetingId)
    {
        var meeting = await meetingService.GetMeetingDtoById(meetingId);
        if (meeting == null)
        {
            return NotFound();
        }

        ViewData["GroupId"] = meeting.GroupId;
        return View(meeting);
    }

    /// <summary>
    /// Saves edited meeting if form is valid
    /// </summary>
    /// <param name="id">Meeting id</param>
    /// <param name="meeting">Edited meeting</param>
    /// <returns>Meetings list view if saved successfully; otherwise showes an error message</returns>
    [HttpPost]
    [Authorize(Roles = "Admin, Lecturer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int meetingId, [Bind("MeetingStartDate,MeetingEndDate,Id")] MeetingDto meeting)
    {
        if (meetingId != meeting.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await meetingService.UpdateMeeting(meeting);
            return RedirectToAction("Details", "Groups", new { id = meeting.GroupId });
        }

        var groups = await groupService.GetAllGroups();
        ViewData["GroupId"] = new SelectList(groups, "Id", "Id", meeting.GroupId);
        return View(meeting);
    }

    /// <summary>
    /// Gets view with meeting delete panel
    /// </summary>
    /// <param name="id">Meeting id</param>
    /// <returns>Meeting delete panel view</returns>
    [Authorize(Roles = "Admin, Lecturer")]
    [HttpGet]
    public async Task<IActionResult> Delete(int meetingId)
    {
        var meeting = await meetingService.GetMeetingDtoById(meetingId);
        if (meeting == null)
        {
            return NotFound();
        }

        return View(meeting);
    }

    /// <summary>
    /// Deletes meeting by id
    /// </summary>
    /// <param name="id">Meeting id</param>
    /// <returns>Meetings list view</returns>
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin, Lecturer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int meetingId)
    {
        var meeting = await meetingService.GetMeetingDtoById(meetingId) ?? throw new NullReferenceException("Meeting doesn't exist");
        var groupId = meeting.GroupId;

        await meetingService.DeleteMeeting(meetingId);

        return RedirectToAction("Details", "Groups", new { groupId });
    }

    /// <summary>
    /// Gets view with attendance code panel
    /// </summary>
    /// <param name="meetingId">Meeting id</param>
    /// <returns>View with attendance code panel</returns>
    [HttpGet]
    [Authorize(Roles = "Admin, Lecturer")]
    [Route("Meetings/AttendanceCheck/{meetingId}")]
    public async Task<IActionResult> AttendanceCheck(int meetingId)
    {
        var meeting = await meetingService.GetMeetingDtoById(meetingId);

        ViewData["DecryptedCode"] = await meetingService.GetMeetingDecryptedCode(meetingId);
        return View(meeting);
    }

    /// <summary>
    /// Change status of attendance code checking, generates and sets the new code if check started
    /// </summary>
    /// <param name="meetingId"></param>
    /// <param name="checkValue"></param>
    /// <returns><c>true</c> if status seted successfully</returns>
    [HttpPost]
    [Authorize(Roles = "Admin, Lecturer")]
    [Route("Meetings/AttendanceCheck")]
    public async Task<bool> AttendanceCheckRealesed(int meetingId, bool checkValue)
    {
        await meetingService.SwitchAttendanceCheckStatus(meetingId, checkValue);

        return true;
    }
}