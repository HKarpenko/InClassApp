using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InClassApp.Models.Entities;
using InClassApp.Repositories;
using InClassApp.Helpers.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace InClassApp.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IPresenceRecordRepository _presenceRecordRepository;
        private readonly IAttendanceCodeManager _attendanceCodeManager;
        private readonly UserManager<AppUser> _userManager;

        public MeetingsController(IMeetingRepository meetingRepository, IGroupRepository groupRepository,
            IPresenceRecordRepository presenceRecordRepository, IAttendanceCodeManager attendanceCodeManager,
            IServiceProvider serviceProvider)
        {
            _meetingRepository = meetingRepository;
            _groupRepository = groupRepository;
            _presenceRecordRepository = presenceRecordRepository;
            _attendanceCodeManager = attendanceCodeManager;
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        }

        // GET: Meetings/Details/5
        [Authorize(Roles = "Admin, Lecturer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _meetingRepository.GetById((int)id);
            if (meeting == null)
            {
                return NotFound();
            }

            ViewData["Records"] = await _presenceRecordRepository.GetPresenceRecordsByMeetingId((int)id);
            ViewData["DecryptedCode"] = _attendanceCodeManager.GetDecryptedCode(meeting.LastlyGeneratedCheckCode, meeting.LastlyGeneratedCodeIV);

            return View(meeting);
        }

        // GET: Meetings/DetailsStudent/5
        [Authorize(Roles = "Admin, Student")]
        public async Task<IActionResult> DetailsStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _meetingRepository.GetById((int)id);
            if (meeting == null)
            {
                return NotFound();
            }

            ViewData["CurrentUserStatus"] = false;
            return View(meeting);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Student")]
        [Route("Meetings/ValidateCode")]
        public async Task<bool> CodeValidationRealesed(int meetingId, string providedCode)
        {
            var meeting = await _meetingRepository.GetById(meetingId);
            var decryptedCode = _attendanceCodeManager.GetDecryptedCode(meeting.LastlyGeneratedCheckCode, meeting.LastlyGeneratedCodeIV);

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var currentStudentId = 1;
            if (decryptedCode.Equals(providedCode) && meeting.IsAttendanceCheckLaunched)
            {
                var presenceRecordToUpdate = meeting.PresenceRecords?.FirstOrDefault(x => x.StudentId == currentStudentId);
                if (presenceRecordToUpdate == null)
                {
                    NotFound();
                }
                presenceRecordToUpdate.Status = true;
                await _presenceRecordRepository.Update(presenceRecordToUpdate);
            }

            return true;
        }

        // GET: Meetings/Create/2
        [Authorize(Roles = "Admin, Lecturer")]
        [HttpGet("Create/{groupId}")]
        public IActionResult Create(int? groupId)
        {
            if (groupId == null)
            {
                return NotFound();
            }

            ViewData["GroupId"] = (int)groupId;
            return View();
        }

        // POST: Meetings/Create/3
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create/{groupId}")]
        [Authorize(Roles = "Admin, Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int groupId, [Bind("MeetingStartDate,MeetingEndDate,GroupId,Id")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                await _meetingRepository.Add(meeting);

                var meetingStudents = (await _groupRepository.GetById(groupId)).StudentGroupRelations.Select(x => x.Student);
                foreach(var student in meetingStudents)
                {
                    var presenceRecord = new PresenceRecord
                    {
                        MeetingId = meeting.Id,
                        StudentId = student.Id,
                        Status = false
                    };

                    await _presenceRecordRepository.Add(presenceRecord);
                }
                return RedirectToAction("Details", "Groups", new { id = groupId });
            }

            ViewData["GroupId"] = groupId;
            return View(meeting);
        }

        // GET: Meetings/Edit/5
        [Authorize(Roles = "Admin, Lecturer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _meetingRepository.GetById((int)id);
            if (meeting == null)
            {
                return NotFound();
            }

            var groups = await _groupRepository.GetAll();
            ViewData["GroupId"] = meeting.GroupId;
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MeetingStartDate,MeetingEndDate,Id")] Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    meeting.GroupId = (await _meetingRepository.GetByIdAsNoTracking(id)).GroupId;
                    await _meetingRepository.Update(meeting);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await MeetingExists(meeting.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Groups", new { id = meeting.GroupId });
            }

            var groups = await _groupRepository.GetAll();
            ViewData["GroupId"] = new SelectList(groups, "Id", "Id", meeting.GroupId);
            return View(meeting);
        }

        // GET: Meetings/Delete/5
        [Authorize(Roles = "Admin, Lecturer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _meetingRepository.GetById((int)id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meeting = await _meetingRepository.GetById(id);
            var groupId = meeting.GroupId;

            var meetingPresenceRecordIds = (await _presenceRecordRepository.GetAll())
                .Where(x => x.MeetingId == meeting.Id)
                .Select(x => x.Id);

            foreach(var recordId in meetingPresenceRecordIds)
            {
                await _presenceRecordRepository.Delete(recordId);
            }

            await _meetingRepository.Delete(id);

            return RedirectToAction("Details", "Groups", new { id = groupId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Lecturer")]
        [Route("Meetings/AttendanceCheck/{meetingId}")]
        public async Task<IActionResult> AttendanceCheck(int meetingId)
        {
            var meeting = await _meetingRepository.GetByIdAsNoTracking(meetingId);

            ViewData["DecryptedCode"] = _attendanceCodeManager.GetDecryptedCode(meeting.LastlyGeneratedCheckCode, meeting.LastlyGeneratedCodeIV);
            return View(meeting);
        }

        // POST: Meetings/AttendanceCheckRealesed
        [HttpPost]
        [Authorize(Roles = "Admin, Lecturer")]
        [Route("Meetings/AttendanceCheck")]
        public async Task<bool> AttendanceCheckRealesed(int meetingId, bool checkValue)
        {
            var meeting = await _meetingRepository.GetById(meetingId);
            if (checkValue)
            {
                meeting.IsAttendanceCheckLaunched = true;
                var generatedCode = _attendanceCodeManager.CreateAttendanceCode();
                _attendanceCodeManager.EncryptMeeting(meeting, generatedCode);

                await _meetingRepository.Update(meeting);
            }
            else
            {
                meeting.IsAttendanceCheckLaunched = false;
                await _meetingRepository.Update(meeting);
            }
            return true;
        }

        private async Task<bool> MeetingExists(int id)
        {
            var groups = await _meetingRepository.GetAll();
            return groups.Any(e => e.Id == id);
        }
    }
}
