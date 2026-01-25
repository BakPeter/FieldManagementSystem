namespace FieldManagementSystem.Services.Field.Infrastructure.types;

public class FieldValidationException : Exception
{
    public IEnumerable<string> ValidationErrors { get; }

    public FieldValidationException(IEnumerable<string> validationErrors) : base("User validation failed.")
    {
        ValidationErrors = validationErrors;
    }
}