using System.Text.RegularExpressions;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Infrastructure.Services.Validation.Validators;

public class ControllerNameValidator : IContrlollerValidator
{
    private static readonly Regex EnglishLettersRegex = new("^[a-zA-Z ]+$", RegexOptions.Compiled);

    public bool Validate(ControllerEntity entity, ref IEnumerable<string> validationErrors)
    {
        var errors = validationErrors.ToList();
        if (string.IsNullOrWhiteSpace(entity.Name) || entity.Name.Length <= 1)
        {
            errors.Add("First name must be longer than 1 character.");
        }
        else if (EnglishLettersRegex.IsMatch(entity.Name) is false)
        {
            errors.Add("First name must contain only English letters.");
        }

        if (errors.Count == 0) return true;

        validationErrors = errors;
        return false;
    }
}