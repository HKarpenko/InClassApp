using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InClassApp.Models.Entities;
using InClassApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InClassApp.Controllers
{
    [Authorize(Roles = "Admin, Lecturer")]
    public class PresenceRecordsController : Controller
    {
        private readonly IPresenceRecordRepository _presenceRecordRepository;

        public PresenceRecordsController(IPresenceRecordRepository presenceRecordRepository)
        {
            _presenceRecordRepository = presenceRecordRepository;
        }


        // GET: PresenceRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presenceRecord = await _presenceRecordRepository.GetById((int)id);
            if (presenceRecord == null)
            {
                return NotFound();
            }

            return View(presenceRecord);
        }

        // GET: PresenceRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presenceRecord = await _presenceRecordRepository.GetById((int)id);
            if (presenceRecord == null)
            {
                return NotFound();
            }

            return View(presenceRecord);
        }

        // POST: PresenceRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Status,Id")] PresenceRecord presenceRecord)
        {
            if (id != presenceRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentRecord = await _presenceRecordRepository.GetByIdAsNoTracking(id);
                    presenceRecord.MeetingId = currentRecord.MeetingId;
                    presenceRecord.StudentId = currentRecord.StudentId;

                    await _presenceRecordRepository.Update(presenceRecord);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await PresenceRecordExists(presenceRecord.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Meetings", new { id = presenceRecord.MeetingId });
            }
      
            return View(presenceRecord);
        }

        private async Task<bool> PresenceRecordExists(int id)
        {
            var presenceRecords = await _presenceRecordRepository.GetAll();
            return presenceRecords.Any(e => e.Id == id);
        }
    }
}
