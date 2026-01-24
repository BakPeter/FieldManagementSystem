using FieldManagementSystem.User.Core.Interfaces.Validation;
using FieldManagementSystem.User.Core.Types;
using System.Text.RegularExpressions;

namespace FieldManagementSystem.User.Infrastructure.Services.Validation.Validators;

public class UserEmailValidator : IUserValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public bool Validate(UserEntity entity, ref IEnumerable<string> validationErrors)
    {
        var errors = validationErrors.ToList();

        if (string.IsNullOrWhiteSpace(entity.Email))
        {
            errors.Add("Email cannot be empty.");
        }
        else if (!EmailRegex.IsMatch(entity.Email))
        {
            errors.Add("Email format is invalid.");
        }

        validationErrors = errors;
        return !errors.Any();
    }
}