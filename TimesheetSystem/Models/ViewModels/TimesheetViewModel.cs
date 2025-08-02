using System.ComponentModel.DataAnnotations;

namespace TimesheetSystem.Models.ViewModels
{
    public class TimesheetViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime WeekStartDate { get; set; }
        public List<TimesheetEntry> Entries { get; set; } = new();
        public Dictionary<string, decimal> ProjectTotals { get; set; } = new();
        public decimal WeekTotal => Entries.Sum(e => e.Hours);
    }

    public class TimesheetEntryCreateModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ProjectId { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Range(0.1, 24.0)]
        public decimal Hours { get; set; }

        public string? Description { get; set; }
    }
}