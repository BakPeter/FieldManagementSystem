using System.Text.RegularExpressions;
using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Core.Types;

namespace FieldManagementSystem.Services.Field.Infrastructure.Services.Validation.Validators;

public class FieldNameValidator : IFieldValidator
{
    private static readonly Regex EnglishLettersRegex = new("^[a-zA-Z ]+$", RegexOptions.Compiled);

    public bool Validate(FieldEntity entity, ref IEnumerable<string> validationErrors)
    {
        var errors = validationErrors.ToList();
        if (string.IsNullOrWhiteSpace(entity.Name) || entity.Name.Length <= 1)
        {
            errors.Add("Field Name must be longer than 1 character.");
        }
        else if (EnglishLettersRegex.IsMatch(entity.Name) is false)
        {
            errors.Add("Field Name must contain only English letters and ' '.");
        }
        
        if (errors.Count == 0) return true;

        validationErrors = errors;
        return false;
    }
}