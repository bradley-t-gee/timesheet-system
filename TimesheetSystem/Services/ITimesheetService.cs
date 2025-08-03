using TimesheetSystem.Models;
namespace TimesheetSystem.Services
{
    public interface ITimesheetService
    {
        Task<TimesheetEntry> AddEntryAsync(TimesheetEntry entry);
        Task<TimesheetEntry?> GetEntryAsync(int id);
        Task<TimesheetEntry?> UpdateEntryAsync(int id, TimesheetEntry entry);
        Task<bool> DeleteEntryAsync(int id);
        Task<List<TimesheetEntry>> GetEntriesForUserAndWeekAsync(string userId, DateTime weekStart);
        Task<Dictionary<string, decimal>> GetProjectTotalsForUserAndWeekAsync(string userId, DateTime weekStart);
        Task<bool> HasDuplicateEntryAsync(string userId, string projectId, DateTime date, int? excludeId = null);
        Task<List<string>> GetAllUserIdsAsync();
        Task<(DateTime? earliest, DateTime? latest)> GetDateRangeAsync();
    }
}
