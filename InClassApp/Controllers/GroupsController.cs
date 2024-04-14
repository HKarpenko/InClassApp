using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Domain.Models.Dtos;

namespace InClassApp.Controllers
{
    /// <summary>
    /// Controller for group management
    /// </summary>
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILecturersRepository _lecturersRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IPresenceRecordRepository _presenceRecordRepository;
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Groups controller constructor
        /// </summary>
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

        /// <summary>
        /// Gets view with all groups for current user
        /// </summary>
        /// <returns>Groups list view</returns>
        [HttpGet]
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
                ViewData["CurrentRole"] = "Admin";
                return View(groups);
            }

            if (currentUserRoles.Any(x => x == "Lecturer"))
            {
                var currentLecturer = await _lecturersRepository.GetLecturerByUserId(currentUser.Id);
                groups = groups.Where(x => x.LecturerGroupRelations.Any(r => r.LecturerId == currentLecturer.Id)).ToList();

                ViewData["CurrentRole"] = "Lecturer";
            }
            else if (currentUserRoles.Any(x => x == "Student"))
            {
                var currentStudent = await _studentRepository.GetStudentByUserId(currentUser.Id);
                groups = groups.Where(x => x.StudentGroupRelations.Any(r => r.StudentId == currentStudent.Id)).ToList();

                ViewData["CurrentRole"] = "Student";
            }

            return View(groups);
        }

        /// <summary>
        /// Gets view with group details
        /// </summary>
        /// <param name="id">Group id</param>
        /// <returns>Group details view</returns>
        [HttpGet]
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
            if (await IsCurrentUserHaveRightsToEditGroup((int)id))
            {
                ViewData["Rights"] = "Edit";
            }
            else
            {
                ViewData["Rights"] = "Read";
            }

            var @group = await _groupRepository.GetById((int)id);
            if (@group == null)
            {
                return NotFound();
            }

            ViewData["Meetings"] = await _meetingRepository.GetMeetingsByGroupId((int)id);
            return View(@group);
        }

        /// <summary>
        /// Gets view with group create form
        /// </summary>
        /// <returns>Create group view</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Subjects"] = await _subjectRepository.GetAll();
            ViewData["Lecturers"] = await _lecturersRepository.GetAll();
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
                Group newGroup = new Group
                {
                    Name = groupDto.Name,
                    StartDate = groupDto.StartDate,
                    EndDate = groupDto.EndDate,
                    StudiesSemestr = groupDto.StudiesSemestr,
                    SubjectId = groupDto.SubjectId
                };
                await _groupRepository.Add(newGroup);

                foreach (var lecturerId in groupDto.LecturersIds)
                {
                    await _groupRepository.AddLecturerGroupRelation(lecturerId, newGroup.Id);
                }
                return RedirectToAction("Index");
            }


            ViewData["Subjects"] = await _subjectRepository.GetAll();
            ViewData["Lecturers"] = await _lecturersRepository.GetAll();
            return View();
        }

        /// <summary>
        /// Gets view with group edit form
        /// </summary>
        /// <param name="id">Group id</param>
        /// <returns>Edit group view</returns>
        [Authorize(Roles = "Admin, Lecturer")]
        [HttpGet]
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
            SaveGroupDto groupDto = new SaveGroupDto
            {
                Id = group.Id,
                Name = group.Name,
                StudiesSemestr = group.StudiesSemestr,
                StartDate = group.StartDate,
                EndDate = group.EndDate,
                SubjectId = group.SubjectId,
                LecturersIds = group.LecturerGroupRelations.Select(x => x.LecturerId).ToList()
            };

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            ViewData["Role"] = currentUserRoles.FirstOrDefault();
            ViewData["Subjects"] = await _subjectRepository.GetAll();
            ViewData["Lecturers"] = await _lecturersRepository.GetAll();
            return View(groupDto);
        }

        /// <summary>
        /// Saves edited group if form is valid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupDto">Group dto to save</param>
        /// <returns>Group list view if saved successfully; otherwise showes an error message</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, StudiesSemestr, SubjectId, LecturersIds, StartDate, EndDate")] SaveGroupDto groupDto)
        {
            if (id != groupDto.Id)
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
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
                    var role = currentUserRoles.FirstOrDefault();

                    var group = await _groupRepository.GetById(id);
                    group.Name = groupDto.Name;
                    group.StudiesSemestr = groupDto.StudiesSemestr;
                    group.StartDate = groupDto.StartDate;
                    group.EndDate = groupDto.EndDate;
                    if(role == "Admin")
                    {
                        var currentLecturerIds = group.LecturerGroupRelations.Select(x => x.LecturerId).ToList();
                        await DeltaGroupLecturerRelations(id, currentLecturerIds, groupDto.LecturersIds);
                        group.SubjectId = groupDto.SubjectId;
                        
                    }

                    await _groupRepository.Update(group);
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
                return RedirectToAction(nameof(Index));
            }

            ViewData["Subjects"] = await _subjectRepository.GetAll();
            ViewData["Lecturers"] = await _lecturersRepository.GetAll();
            return View(groupDto);
        }

        /// <summary>
        /// Gets view with group delete panel
        /// </summary>
        /// <param name="id">Group id</param>
        /// <returns>Group delete panel view</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
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

        /// <summary>
        /// Deletes group by id
        /// </summary>
        /// <param name="id">Group id</param>
        /// <returns>Groups list view</returns>
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _groupRepository.GetById(id);

            await _groupRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Gets view with group students list
        /// </summary>
        /// <param name="id">Group id</param>
        /// <returns>Group students list view</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, Lecturer")]
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

        /// <summary>
        /// Gets view with adding student to list form
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <returns>Add student to group view</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, Lecturer")]
        [Route("Groups/AddStudent/{groupId:int?}")]
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

        /// <summary>
        /// Adds student by index to group
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <param name="index">Students index</param>
        /// <returns>Group students list view if saved successfully; otherwise shows an error message</returns>
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

        /// <summary>
        /// Gets group student remove panel
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <param name="studentId">Student id</param>
        /// <returns>Group student panel view</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, Lecturer")]
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

        /// <summary>
        /// Removes student form group
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <param name="studentId">Student id</param>
        /// <returns>Group students list view</returns>
        [HttpPost, ActionName("RemoveStudent")]
        [Authorize(Roles = "Admin, Lecturer")]
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
                return currentLecturer.LecturerGroupRelations.Any(r => r.GroupId == groupId);
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
                return currentLecturer.LecturerGroupRelations.Any(r => r.GroupId == groupId);
            }
            return false;
        }

        private async Task<bool> DeltaGroupLecturerRelations(int groupId, ICollection<int> currentLecturerIds, ICollection<int> newLecturerIds)
        {

            foreach (var currentLecturerId in currentLecturerIds)
            {
                await _groupRepository.DeleteLecturerGroupRelation(currentLecturerId, groupId);
            }
            if (newLecturerIds != null)
            {
                foreach (var newLecturerId in newLecturerIds)
                {
                    await _groupRepository.AddLecturerGroupRelation(newLecturerId, groupId);
                }
            }
            return true;
        }
    }
}
