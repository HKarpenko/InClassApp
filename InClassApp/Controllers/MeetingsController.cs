using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InClassApp.Models.Entities;
using InClassApp.Repositories;

namespace InClassApp.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IGroupRepository _groupRepository;

        public MeetingsController(IMeetingRepository meetingRepository, IGroupRepository groupRepository)
        {
            _meetingRepository = meetingRepository;
            _groupRepository = groupRepository;
        }

        // GET: Meetings/Details/5
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

            return View(meeting);
        }

        // GET: Meetings/Create/2
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int groupId, [Bind("MeetingStartDate,MeetingEndDate,GroupId,Id")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                await _meetingRepository.Add(meeting);
                return RedirectToAction("Details", "Groups", new { id = groupId });
            }

            ViewData["GroupId"] = groupId;
            return View(meeting);
        }

        // GET: Meetings/Edit/5
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupId = (await _meetingRepository.GetById(id)).GroupId;
            await _meetingRepository.Delete(id);
            return RedirectToAction("Details", "Groups", new { id = groupId });
        }

        private async Task<bool> MeetingExists(int id)
        {
            var groups = await _meetingRepository.GetAll();
            return groups.Any(e => e.Id == id);
        }
    }
}
