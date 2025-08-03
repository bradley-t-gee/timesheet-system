using System.ComponentModel.DataAnnotations;
using TimesheetSystem.ValidationAttributes;

namespace TimesheetSystem.Models.ViewModels
{
    public class TimesheetEntryCreateModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ProjectId { get; set; } = string.Empty;

        [Required]
        [NoFutureDate(ErrorMessage = "Date cannot be in the future.")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Range(0.1, 24.0)]
        public decimal Hours { get; set; }

        public string? Description { get; set; }
    }
}