using System.ComponentModel.DataAnnotations;
namespace TimesheetSystem.Models
{
    public class TimesheetEntry
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string ProjectId { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [Range(0.1, 24.0, ErrorMessage = "Hours must be between 0.1 and 24.0")]
        public decimal Hours { get; set; }
        public string? Description { get; set; }
    }
}