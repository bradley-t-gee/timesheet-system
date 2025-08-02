using Microsoft.AspNetCore.Mvc;
using TimesheetSystem.Models;
using TimesheetSystem.Models.ViewModels;
using TimesheetSystem.Services;
namespace TimesheetSystem.Controllers
{
    public class TimesheetController : Controller
    {
        private readonly ITimesheetService _timesheetService;
        public TimesheetController(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }
        public async Task<IActionResult> Index(string userId = "user1", DateTime? weekStart = null)
        {
            weekStart ??= TimesheetService.GetWeekStart(DateTime.Today);
            var entries = await _timesheetService.GetEntriesForUserAndWeekAsync(userId, weekStart.Value);
            var projectTotals = await _timesheetService.GetProjectTotalsForUserAndWeekAsync(userId, weekStart.Value);
            var viewModel = new TimesheetViewModel
            {
                UserId = userId,
                WeekStartDate = weekStart.Value,
                Entries = entries,
                ProjectTotals = projectTotals
            };
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create(string userId = "user1")
        {
            var model = new TimesheetEntryCreateModel
            {
                UserId = userId,
                Date = DateTime.Today
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimesheetEntryCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var entry = new TimesheetEntry
                {
                    UserId = model.UserId,
                    ProjectId = model.ProjectId,
                    Date = model.Date,
                    Hours = model.Hours,
                    Description = model.Description
                };
                await _timesheetService.AddEntryAsync(entry);
                return RedirectToAction(nameof(Index), new { userId = model.UserId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entry = await _timesheetService.GetEntryAsync(id);
            if (entry == null)
            {
                return NotFound();
            }
            var model = new TimesheetEntryCreateModel
            {
                UserId = entry.UserId,
                ProjectId = entry.ProjectId,
                Date = entry.Date,
                Hours = entry.Hours,
                Description = entry.Description
            };
            ViewBag.EntryId = id;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TimesheetEntryCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EntryId = id;
                return View(model);
            }
            try
            {
                var entry = new TimesheetEntry
                {
                    UserId = model.UserId,
                    ProjectId = model.ProjectId,
                    Date = model.Date,
                    Hours = model.Hours,
                    Description = model.Description
                };
                var updatedEntry = await _timesheetService.UpdateEntryAsync(id, entry);
                if (updatedEntry == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index), new { userId = model.UserId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.EntryId = id;
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string userId)
        {
            await _timesheetService.DeleteEntryAsync(id);
            return RedirectToAction(nameof(Index), new { userId });
        }
    }
}