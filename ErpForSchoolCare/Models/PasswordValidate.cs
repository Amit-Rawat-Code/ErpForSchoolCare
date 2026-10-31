using System.ComponentModel.DataAnnotations;

namespace ErpForSchoolCare.Models
{
    public class PasswordValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var password = value?.ToString();

            if (string.IsNullOrWhiteSpace(password))
                return new ValidationResult("Password is required.");

            if (password.Length < 8)
                return new ValidationResult("Password must be at least 8 characters long.");

            if (!password.Any(char.IsUpper))
                return new ValidationResult("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                return new ValidationResult("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                return new ValidationResult("Password must contain at least one digit.");

            if (!password.Any(ch => "@$!%*?&".Contains(ch)))
                return new ValidationResult("Password must contain at least one special character.");

            return ValidationResult.Success;
        }
    }

}
