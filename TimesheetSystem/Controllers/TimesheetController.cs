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

        [HttpGet]
        public IActionResult Search()
        {
            // Return empty search form on initial load
            var searchModel = new TimesheetSearchViewModel
            {
            };

            return View(searchModel);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(TimesheetSearchViewModel searchModel)
        {
            if (!ModelState.IsValid)
            {
                return View(searchModel);
            }

            // Ensure we're using the start of the week
            var weekStart = TimesheetService.GetWeekStart(searchModel.WeekStartDate);

            // Get filtered data
            var entries = await _timesheetService.GetEntriesForUserAndWeekAsync(searchModel.UserId, weekStart);
            var projectTotals = await _timesheetService.GetProjectTotalsForUserAndWeekAsync(searchModel.UserId, weekStart);

            // Update search model with results
            searchModel.WeekStartDate = weekStart;
            searchModel.Entries = entries;
            searchModel.ProjectTotals = projectTotals;

            // Mark that a search was performed
            searchModel.HasSearched = true;

            return View(searchModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new TimesheetEntryCreateModel
            {
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
                return RedirectToAction(nameof(Search), new
                {
                    userId = model.UserId,
                    weekStartDate = model.Date.ToString("yyyy-MM-dd")
                });
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
                return RedirectToAction(nameof(Search), new
                {
                    userId = model.UserId,
                    weekStartDate = model.Date.ToString("yyyy-MM-dd")
                });
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
            var weekStartDate = Request.Form["weekStartDate"].ToString();
            return RedirectToAction(nameof(Search), new
            {
                userId,
                weekStartDate
            });
        }
    }
}