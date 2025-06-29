using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Domain.Models.Dtos;

namespace InClassApp.Controllers;

/// <summary>
/// Controller for subject management
/// </summary>
[Authorize(Roles = "Admin")]
public class SubjectsController(
    IGroupService groupService, 
    ISubjectService subjectService) : Controller
{
    /// <summary>
    /// Gets view with all subjects
    /// </summary>
    /// <returns>Subjects list view</returns>
    [HttpGet]
    public async Task<IActionResult> SubjectsList()
    {
        var subjects = await subjectService.GetAllSubjectDtos();
        return View(subjects);
    }

    /// <summary>
    /// Gets view with subject details
    /// </summary>
    /// <param name="id">Subject id</param>
    /// <returns>Subject details view</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var subject = await subjectService.GetSubjectDtoById(id);
        if (subject == null)
        {
            return NotFound();
        }

        ViewData["Groups"] = await groupService.GetGroupDtosBySubjectId(id);
        return View(subject);
    }

    /// <summary>
    /// Gets view with subject create form
    /// </summary>
    /// <returns>Create subject view</returns>
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Saves given subject if it is valid
    /// </summary>
    /// <param name="subject">Subject to add</param>
    /// <returns>Subjects list view if saved successfully; otherwise showes an error message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Code,Id")] SaveSubjectDto subject)
    {
        if (ModelState.IsValid)
        {
            await subjectService.CreateSubject(subject);
            return RedirectToAction(nameof(SubjectsList));
        }
        return View();
    }

    /// <summary>
    /// Gets view with subject edit form
    /// </summary>
    /// <param name="id">Subject id</param>
    /// <returns>Edit subject view</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var subject = await subjectService.GetSubjectDtoById(id);
        if (subject == null)
        {
            return NotFound();
        }
        return View(subject);
    }

    /// <summary>
    /// Saves edited subject if form is valid
    /// </summary>
    /// <param name="id">Subject id</param>
    /// <param name="subject">Subject to save</param>
    /// <returns>Subjects list view if saved successfully; otherwise showes an error message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Code,Id")] SaveSubjectDto subject)
    {
        if (id != subject.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await subjectService.UpdateSubject(subject);
            return RedirectToAction(nameof(SubjectsList));
        }
        return View(subject);
    }

    /// <summary>
    /// Gets view with subject delete panel
    /// </summary>
    /// <param name="id">Subject id</param>
    /// <returns>Subject delete panel view</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await subjectService.GetSubjectDtoById(id);
        if (subject == null)
        {
            return NotFound();
        }

        return View(subject);
    }

    /// <summary>
    /// Deletes subject by id
    /// </summary>
    /// <param name="id">Subject id</param>
    /// <returns>Subjects list view</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {

        await subjectService.DeleteSubject(id);
        return RedirectToAction(nameof(SubjectsList));
    }
}
