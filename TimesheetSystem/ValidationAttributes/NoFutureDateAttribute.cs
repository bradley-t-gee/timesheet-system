using System;
using System.ComponentModel.DataAnnotations;

namespace TimesheetSystem.ValidationAttributes
{
    public class NoFutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateValue)
            {
                return dateValue.Date <= DateTime.Today;
            }
            return true; // Nulls handled by [Required]
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be in the future.";
        }
    }
}