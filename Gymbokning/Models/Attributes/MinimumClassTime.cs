using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Models.Attributes
{
    public class MinimumClassTime : ValidationAttribute
    {
        private readonly TimeSpan _minValue;

        public MinimumClassTime(int hours, int minutes)
        {
            _minValue = new TimeSpan(hours, minutes, 0); // Create a TimeSpan from hours and minutes
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is TimeSpan timeSpanValue)
            {
                if (timeSpanValue < _minValue)
                {
                    return new ValidationResult($"The duration must be at least {_minValue.Hours} hours and {_minValue.Minutes} minutes.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
