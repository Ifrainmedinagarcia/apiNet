using System.ComponentModel.DataAnnotations;

namespace API.Validations;

public class FirstLetterInUpperCase : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        var firstLetter = value.ToString()?[0].ToString();

        return firstLetter != null && firstLetter != firstLetter
            .ToUpper() ? 
            new ValidationResult("The first Letter should be in upperCase") : 
            ValidationResult.Success;
    }
}