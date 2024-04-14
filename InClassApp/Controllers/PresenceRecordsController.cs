using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InClassApp.Controllers
{
    /// <summary>
    /// Controller for presence records management
    /// </summary>
    [Authorize(Roles = "Admin, Lecturer")]
    public class PresenceRecordsController : Controller
    {
        private readonly IPresenceRecordRepository _presenceRecordRepository;

        /// <summary>
        /// Presence records controller constructor
        /// </summary>
        public PresenceRecordsController(IPresenceRecordRepository presenceRecordRepository)
        {
            _presenceRecordRepository = presenceRecordRepository;
        }

        /// <summary>
        /// Gets view with the presence record details
        /// </summary>
        /// <param name="id">Presence record id</param>
        /// <returns>Presence record details view</returns>
        [HttpGet]
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

        /// <summary>
        /// Gets view with presence record edit form
        /// </summary>
        /// <param name="id">Presence record id</param>
        /// <returns>Edit Presence record view</returns>
        [HttpGet]
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

        /// <summary>
        /// Saves edited group if form is valid
        /// </summary>
        /// <param name="id">Presence record id</param>
        /// <param name="presenceRecord">Presence record to save</param>
        /// <returns>Meeting details view if saved successfully; otherwise showes an error message</returns>
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
