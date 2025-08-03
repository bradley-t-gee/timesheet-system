using System.ComponentModel.DataAnnotations;

namespace TimesheetSystem.Models.ViewModels
{
    public class TimesheetSearchViewModel
    {
        [Display(Name = "User ID")]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Week Starting")]
        [DataType(DataType.Date)]
        public DateTime WeekStartDate { get; set; } = DateTime.Today;

        // Results
        public List<TimesheetEntry> Entries { get; set; } = new();
        public Dictionary<string, decimal> ProjectTotals { get; set; } = new();
        public decimal WeekTotal => Entries.Sum(e => e.Hours);

        // Helper property for displaying week range
        public string WeekRange => $"{WeekStartDate:dd MMM} - {WeekStartDate.AddDays(6):dd MMM, yyyy}";
        public bool HasSearched { get; set; } = false;
    }
}