using System;
using System.ComponentModel.DataAnnotations;

namespace Cs_kt3
{
    public class AgeValidationAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public AgeValidationAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > today.AddYears(-age)) age--;

                if (age < _minimumAge)
                {
                    return new ValidationResult($"Регистрация запрещена для пользователей младше {_minimumAge} лет");
                }
            }

            return ValidationResult.Success;
        }
    }
}