using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Domain.Models.Dtos;
using Infrastructure.Interfaces;

namespace InClassApp.Controllers
{
    /// <summary>
    /// Controller for subject management
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class SubjectsController : Controller
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ISubjectService _subjectService;

        /// <summary>
        /// Subjects controller constructor
        /// </summary>
        public SubjectsController(IGroupRepository groupRepository, ISubjectService subjectService)
        {
            _subjectService = subjectService;
            _groupRepository = groupRepository;
        }

        /// <summary>
        /// Gets view with all subjects
        /// </summary>
        /// <returns>Subjects list view</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var subjectsAsync = await _subjectService.GetAllSubjects();
            return View(subjectsAsync);
        }

        /// <summary>
        /// Gets view with subject details
        /// </summary>
        /// <param name="id">Subject id</param>
        /// <returns>Subject details view</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var subject = id == null ? null : await _subjectService.GetSubjectById((int)id);
            if (subject == null)
            {
                return NotFound();
            }

            ViewData["Groups"] = await _groupRepository.GetGroupsBySubjectId((int)id).ToListAsync();
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
                await _subjectService.CreateSubject(subject);
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        /// <summary>
        /// Gets view with subject edit form
        /// </summary>
        /// <param name="id">Subject id</param>
        /// <returns>Edit subject view</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var subject = id == null ? null : await _subjectService.GetSubjectById((int)id);
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _subjectService.UpdateSubject(subject);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await SubjectExists(subject.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        /// <summary>
        /// Gets view with subject delete panel
        /// </summary>
        /// <param name="id">Subject id</param>
        /// <returns>Subject delete panel view</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var subject = id == null ? null : await _subjectService.GetSubjectById((int)id);
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

            await _subjectService.DeleteSubject(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SubjectExists(int id)
        {
            var subjects = await _subjectService.GetAllSubjects();
            return subjects.Any(e => e.Id == id);
        }
    }
}
