using TimesheetSystem.Models;
using TimesheetSystem.Services;
using System.Globalization;
namespace TimesheetSystem.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly List<TimesheetEntry> _entries = new();
        private int _nextId = 1;
        public Task<TimesheetEntry> AddEntryAsync(TimesheetEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            // Check for duplicates (stretch requirement)
            if (HasDuplicateEntryAsync(entry.UserId, entry.ProjectId, entry.Date).Result)
            {
                throw new InvalidOperationException("An entry for this user, project, and date already exists.");
            }
            entry.Id = _nextId++;
            _entries.Add(entry);
            return Task.FromResult(entry);
        }
        public Task<TimesheetEntry?> GetEntryAsync(int id)
        {
            var entry = _entries.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(entry);
        }
        public Task<TimesheetEntry?> UpdateEntryAsync(int id, TimesheetEntry updatedEntry)
        {
            var existingEntry = _entries.FirstOrDefault(e => e.Id == id);
            if (existingEntry == null)
                return Task.FromResult<TimesheetEntry?>(null);
            // Check for duplicates when updating (exclude current entry)
            if (HasDuplicateEntryAsync(updatedEntry.UserId, updatedEntry.ProjectId, updatedEntry.Date, id).Result)
            {
                throw new InvalidOperationException("An entry for this user, project, and date already exists.");
            }
            existingEntry.UserId = updatedEntry.UserId;
            existingEntry.ProjectId = updatedEntry.ProjectId;
            existingEntry.Date = updatedEntry.Date;
            existingEntry.Hours = updatedEntry.Hours;
            existingEntry.Description = updatedEntry.Description;
            return Task.FromResult<TimesheetEntry?>(existingEntry);
        }
        public Task<bool> DeleteEntryAsync(int id)
        {
            var entry = _entries.FirstOrDefault(e => e.Id == id);
            if (entry != null)
            {
                _entries.Remove(entry);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        public Task<List<TimesheetEntry>> GetEntriesForUserAndWeekAsync(string userId, DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(6);
            var entries = _entries
            .Where(e => e.UserId == userId && e.Date >= weekStart && e.Date <= weekEnd)
            .OrderBy(e => e.Date)
            .ToList();
            return Task.FromResult(entries);
        }
        public Task<Dictionary<string, decimal>> GetProjectTotalsForUserAndWeekAsync(string userId, DateTime weekStart)
        {
            var entries = GetEntriesForUserAndWeekAsync(userId, weekStart).Result;
            var projectTotals = entries
            .GroupBy(e => e.ProjectId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Hours));
            return Task.FromResult(projectTotals);
        }
        public Task<bool> HasDuplicateEntryAsync(string userId, string projectId, DateTime date, int? excludeId = null)
        {
            var duplicate = _entries.Any(e =>
            e.UserId == userId &&
            e.ProjectId == projectId &&
            e.Date.Date == date.Date &&
            (excludeId == null || e.Id != excludeId));
            return Task.FromResult(duplicate);
        }
        // Helper method to get start of week (Monday)
        public static DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}