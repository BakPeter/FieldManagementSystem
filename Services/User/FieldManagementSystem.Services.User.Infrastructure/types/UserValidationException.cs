namespace FieldManagementSystem.Services.User.Infrastructure.types;

public class UserValidationException : Exception
{
    public IEnumerable<string> ValidationErrors { get; }

    public UserValidationException(IEnumerable<string> validationErrors) : base("User validation failed.")
    {
        ValidationErrors = validationErrors;
    }
}