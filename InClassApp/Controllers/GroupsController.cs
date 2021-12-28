using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InClassApp.Models.Entities;
using InClassApp.Repositories;

namespace InClassApp.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILecturersRepository _lecturersRepository;
        private readonly IMeetingRepository _meetingRepository;

        public GroupsController(ISubjectRepository subjectRepository, IGroupRepository groupRepository,
            ILecturersRepository lecturersRepository, IMeetingRepository meetingRepository)
        {
            _subjectRepository = subjectRepository;
            _groupRepository = groupRepository;
            _lecturersRepository = lecturersRepository;
            _meetingRepository = meetingRepository;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _groupRepository.GetAll();
            return View(await applicationDbContext);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _groupRepository.GetById((int)id);
            if (@group == null)
            {
                return NotFound();
            }

            ViewData["Meetings"] = await _meetingRepository.GetMeetingsByGroupId((int)id);
            return View(@group);
        }

        // GET: Groups/Create
        public async Task<IActionResult> Create()
        {
            var subjects = await _subjectRepository.GetAll();
            var lecturers = await _lecturersRepository.GetAll();

            ViewData["SubjectId"] = new SelectList(subjects, "Id", "Id");
            ViewData["LecturerId"] = new SelectList(lecturers, "Id", "Id");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StudiesSemestr,SubjectId,LecturerId,StartDate,EndDate,Id")] Group @group)
        {
            if (ModelState.IsValid)
            {
                await _groupRepository.Add(@group);
                return RedirectToAction(nameof(Index));
            }

            var subjects = await _subjectRepository.GetAll();
            var lecturers = await _lecturersRepository.GetAll();

            ViewData["SubjectId"] = new SelectList(subjects, "Id", "Id", @group.SubjectId);
            ViewData["LecturerId"] = new SelectList(lecturers, "Id", "Id", @group.LecturerId);
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _groupRepository.GetById((int)id);
            if (@group == null)
            {
                return NotFound();
            }

            var subjects = await _subjectRepository.GetAll();
            var lecturers = await _lecturersRepository.GetAll();

            ViewData["SubjectId"] = new SelectList(subjects, "Id", "Id", @group.SubjectId);
            ViewData["LecturerId"] = new SelectList(lecturers, "Id", "Id", @group.LecturerId);
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,StudiesSemestr,SubjectId,LecturerId,StartDate,EndDate,Id")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _groupRepository.Update(@group);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await GroupExists(@group.Id)))
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

            var subjects = await _subjectRepository.GetAll();
            var lecturers = await _lecturersRepository.GetAll();

            ViewData["SubjectId"] = new SelectList(subjects, "Id", "Id", @group.SubjectId);
            ViewData["LecturerId"] = new SelectList(lecturers, "Id", "Id", @group.LecturerId);
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _groupRepository.GetById((int)id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _groupRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> GroupExists(int id)
        {
            var groups = await _groupRepository.GetAll();
            return groups.Any(e => e.Id == id);
        }
    }
}
