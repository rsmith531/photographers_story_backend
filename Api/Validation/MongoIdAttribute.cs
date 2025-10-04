using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public class MongoIdAttribute : ValidationAttribute
{
    public MongoIdAttribute()
    {
        ErrorMessage = "Invalid ID format";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // [Required] check
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("ID is required.");
        }

        // [Type] check
        if (value is not string idString)
        {
            return new ValidationResult("ID must be a string.");
        }

        // [RegularExpression] check
        if (ObjectId.TryParse(idString, out _))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage);
    }
}