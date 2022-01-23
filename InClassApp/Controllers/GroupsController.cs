using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InClassApp.Models.Entities;
using InClassApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace InClassApp.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILecturersRepository _lecturersRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IPresenceRecordRepository _presenceRecordRepository;
        private readonly UserManager<AppUser> _userManager;

        public GroupsController(ISubjectRepository subjectRepository, IGroupRepository groupRepository,
            ILecturersRepository lecturersRepository, IMeetingRepository meetingRepository, IStudentRepository studentRepository,
            IPresenceRecordRepository presenceRecordRepository, IServiceProvider serviceProvider)
        {
            _subjectRepository = subjectRepository;
            _groupRepository = groupRepository;
            _lecturersRepository = lecturersRepository;
            _meetingRepository = meetingRepository;
            _studentRepository = studentRepository;
            _presenceRecordRepository = presenceRecordRepository;
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserRoles = currentUser != null ? await _userManager.GetRolesAsync(currentUser) : null;
            if (currentUser == null || currentUserRoles == null)
            {
                return Unauthorized();
            }

            var groups = await _groupRepository.GetAll();
            if (currentUserRoles.Any(x => x == "Admin")) 
            {
                return View(groups);
            }

            if (currentUserRoles.Any(x => x == "Lecturer"))
            {
                var currentLecturer = await _lecturersRepository.GetLecturerByUserId(currentUser.Id);
                groups = groups.Where(x => x.LecturerId == currentLecturer.Id).ToList();
            }
            else if (currentUserRoles.Any(x => x == "Student"))
            {
                var currentStudent = await _studentRepository.GetStudentByUserId(currentUser.Id);
                groups = groups.Where(x => x.StudentGroupRelations.Any(r => r.StudentId == currentStudent.Id)).ToList();
            }

            return View(groups);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!(await IsCurrentUserHaveRightsToReadGroup((int)id)))
            {
                return Unauthorized();
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

            if ( !(await IsCurrentUserHaveRightsToEditGroup((int)id)) )
            {
                return Unauthorized();
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

            if (!(await IsCurrentUserHaveRightsToEditGroup(id)))
            {
                return Unauthorized();
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _groupRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Groups/List/2
        public async Task<IActionResult> StudentsList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!(await IsCurrentUserHaveRightsToEditGroup((int)id)))
            {
                return Unauthorized();
            }

            ViewData["GroupId"] = id;
            var group = await _groupRepository.GetById((int)id);
            return View(group.StudentGroupRelations.Select(x => x.Student));
        }

        [HttpGet]
        [Route("Groups/AddStudent/{groupId:int?}")]
        // GET: Groups/AddStudent/3
        public async Task<IActionResult> AddStudent(int? groupId)
        {
            if (groupId == null)
            {
                return NotFound();
            }

            if (!(await IsCurrentUserHaveRightsToEditGroup((int)groupId)))
            {
                return Unauthorized();
            }

            var addedStudents = (await _groupRepository.GetById((int)groupId)).StudentGroupRelations.Select(r => r.Student);
            var students = (await _studentRepository.GetAll()).Except(addedStudents);
            ViewData["Indecies"] = new SelectList(students, "Index", "Index");
            ViewData["GroupId"] = groupId;

            return View();
        }

        // POST: Students/AddStudent/3
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Groups/AddStudent/{groupId:int?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int? groupId, [Bind("Index")] string index)
        {
            if (groupId == null)
            {
                return NotFound();
            }

            if (!(await IsCurrentUserHaveRightsToEditGroup((int)groupId)))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var student = await _studentRepository.GetStudentByIndex(index);
                var group = await _groupRepository.GetById((int)groupId);
                await _groupRepository.AddStudentGroupRelation(student.Id, group.Id);

                foreach(var meeting in group.Meetings)
                {
                    var presenceRecord = new PresenceRecord
                    {
                        MeetingId = meeting.Id,
                        StudentId = student.Id,
                        Status = false
                    };

                    await _presenceRecordRepository.Add(presenceRecord);
                }

                return RedirectToAction("StudentsList", "Groups", new { id = groupId });
            }

            var students = await _studentRepository.GetAll();
            ViewData["Indecies"] = new SelectList(students, "Index", "Index");
            ViewData["GroupId"] = groupId;

            return View();
        }

        // GET: Groups/RemoveStudent?groupId=3&&studentId=1
        public async Task<IActionResult> RemoveStudent(int? groupId, int? studentId)
        {
            if (groupId == null || studentId == null)
            {
                return NotFound();
            }

            if (!(await IsCurrentUserHaveRightsToEditGroup((int)groupId)))
            {
                return Unauthorized();
            }

            var student = await _studentRepository.GetById((int)studentId);
            if (student == null)
            {
                return NotFound();
            }

            ViewData["GroupId"] = groupId;
            return View(student);
        }

        // POST: Groups/RemoveStudent?groupId=3&&studentId=1
        [HttpPost, ActionName("RemoveStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudentConfirmed(int? groupId, int? studentId)
        {
            if (groupId == null || studentId == null)
            {
                return NotFound();
            }

            if (!(await IsCurrentUserHaveRightsToEditGroup((int)groupId)))
            {
                return Unauthorized();
            }

            var presenceRecordIdsToDelete = (await _presenceRecordRepository.GetAll())
                .Where(x => x.Meeting.GroupId == groupId)
                .Select(x => x.Id);

            foreach(var idToDelete in presenceRecordIdsToDelete)
            {
                await _presenceRecordRepository.Delete(idToDelete);
            }

            await _groupRepository.DeleteStudentGroupRelation((int)studentId, (int)groupId);
            return RedirectToAction("StudentsList", "Groups", new { id = groupId });
        }

        private async Task<bool> GroupExists(int id)
        {
            var groups = await _groupRepository.GetAll();
            return groups.Any(e => e.Id == id);
        }

        private async Task<bool> IsCurrentUserHaveRightsToReadGroup(int groupId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserRoles = currentUser != null ? await _userManager.GetRolesAsync(currentUser) : null;

            if (currentUser == null || currentUserRoles == null || currentUserRoles.Count == 0)
            {
                return false;
            }

            if (currentUserRoles.Any(x => x == "Admin"))
            {
                return true;
            }

            if (currentUserRoles.Any(x => x == "Lecturer") == true)
            {
                var currentLecturer = await _lecturersRepository.GetLecturerByUserId(currentUser.Id);
                return currentLecturer.Groups.Any(r => r.Id == groupId);
            }
            else
            {
                var currentStudent = await _studentRepository.GetStudentByUserId(currentUser.Id);

                return currentStudent.StudentGroupRelations.Any(r => r.GroupId == groupId);
            }
        }

        private async Task<bool> IsCurrentUserHaveRightsToEditGroup(int groupId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserRoles = currentUser != null ? await _userManager.GetRolesAsync(currentUser) : null;

            if (currentUser == null || currentUserRoles == null || currentUserRoles.Count == 0)
            {
                return false;
            }

            if (currentUserRoles.Any(x => x == "Admin"))
            {
                return true;
            }

            if (currentUserRoles.Any(x => x == "Lecturer") == true)
            {
                var currentLecturer = await _lecturersRepository.GetLecturerByUserId(currentUser.Id);
                return currentLecturer.Groups.Any(r => r.Id == groupId);
            }
            return false;
        }
    }
}
