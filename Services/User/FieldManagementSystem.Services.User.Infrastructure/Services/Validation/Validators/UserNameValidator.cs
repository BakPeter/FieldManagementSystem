using FieldManagementSystem.User.Core.Interfaces.Validation;
using FieldManagementSystem.User.Core.Types;
using System.Text.RegularExpressions;

namespace FieldManagementSystem.User.Infrastructure.Services.Validation.Validators;

public class UserNameValidator : IUserValidator
{
    private static readonly Regex EnglishLettersRegex = new("^[a-zA-Z]+$", RegexOptions.Compiled);

    public bool Validate(UserEntity entity, ref IEnumerable<string> validationErrors)
    {
        var errors = validationErrors.ToList();
        if (string.IsNullOrWhiteSpace(entity.FirstName) || entity.FirstName.Length <= 1)
        {
            errors.Add("First name must be longer than 1 character.");
        }
        else if (EnglishLettersRegex.IsMatch(entity.FirstName) is false)
        {
            errors.Add("First name must contain only English letters.");
        }

        if (string.IsNullOrWhiteSpace(entity.LastName) || entity.LastName.Length <= 1)
        {
            errors.Add("Last name must be longer than 1 character.");
        }
        else if (EnglishLettersRegex.IsMatch(entity.LastName) is false)
        {
            errors.Add("Last name must contain only English letters.");
        }

        if (errors.Count == 0) return true;

        validationErrors = errors;
        return false;
    }
}