using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InClassApp.Models.Entities;
using InClassApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InClassApp.Controllers
{
    /// <summary>
    /// Controller for subject management
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class SubjectsController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IGroupRepository _groupRepository;

        /// <summary>
        /// Subjects controller constructor
        /// </summary>
        public SubjectsController(ISubjectRepository subjectRepository, IGroupRepository groupRepository)
        {
            _subjectRepository = subjectRepository;
            _groupRepository = groupRepository;
        }

        /// <summary>
        /// Gets view with all subjects
        /// </summary>
        /// <returns>Subjects list view</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var subjectsAsync = _subjectRepository.GetAll();
            return View(await subjectsAsync);
        }

        /// <summary>
        /// Gets view with subject details
        /// </summary>
        /// <param name="id">Subject id</param>
        /// <returns>Subject details view</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectRepository.GetById((int)id);
            if (subject == null)
            {
                return NotFound();
            }

            ViewData["Groups"] = await _groupRepository.GetGroupsBySubjectId((int)id);
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
        public async Task<IActionResult> Create([Bind("Name,Code,Id")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _subjectRepository.Add(subject);
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
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectRepository.GetById((int)id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Name,Code,Id")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _subjectRepository.Update(subject);
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
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectRepository.GetById((int)id);
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

            await _subjectRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SubjectExists(int id)
        {
            var subjects = await _subjectRepository.GetAll();
            return subjects.Any(e => e.Id == id);
        }
    }
}
