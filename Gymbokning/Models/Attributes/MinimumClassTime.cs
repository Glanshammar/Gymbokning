using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Models.Attributes
{
    public class MinimumClassTime : ValidationAttribute
    {
        private readonly TimeSpan _minValue;

        public MinimumClassTime(int hours, int minutes)
        {
            _minValue = new TimeSpan(hours, minutes, 0);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is TimeSpan timeSpanValue)
            {
                if (timeSpanValue < _minValue)
                {
                    string message = BuildErrorMessage(_minValue);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }

        private string BuildErrorMessage(TimeSpan minValue)
        {
            string hourPart = minValue.Hours > 0 
                ? $"{minValue.Hours} hour" + (minValue.Hours > 1 ? "s" : "") 
                : string.Empty;

            string minutePart = minValue.Minutes > 0 
                ? $"{minValue.Minutes} minute" + (minValue.Minutes > 1 ? "s" : "") 
                : string.Empty;

            if (!string.IsNullOrEmpty(hourPart) && !string.IsNullOrEmpty(minutePart))
            {
                return $"The duration must be at least {hourPart} and {minutePart}.";
            }
            else if (!string.IsNullOrEmpty(hourPart))
            {
                return $"The duration must be at least {hourPart}.";
            }
            else
            {
                return $"The duration must be at least {minutePart}.";
            }
        }
    }
}