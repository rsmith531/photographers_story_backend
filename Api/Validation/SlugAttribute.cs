using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public class SlugAttribute : ValidationAttribute
{
    // alphanumeric + dash chars
    private static readonly Regex _slugRegex = 
        new(@"^[a-zA-Z0-9-]+$", RegexOptions.Compiled);

    public SlugAttribute()
    {
        // default error message
        ErrorMessage = "Invalid slug format";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // [Required] check
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Slug is required.");
        }

        // [Type] check
        if (value is not string slug)
        {
            return new ValidationResult("Slug must be a string.");
        }

        // [RegularExpression] check
        if (!_slugRegex.IsMatch(slug))
        {
            return new ValidationResult(ErrorMessage);
        }
        
        return ValidationResult.Success;
    }
}