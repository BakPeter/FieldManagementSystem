namespace FieldManagementSystem.Services.Controller.Infrastructure.types;

public class ControllerValidationException : Exception
{
    public IEnumerable<string> ValidationErrors { get; }

    public ControllerValidationException(IEnumerable<string> validationErrors) : base("User validation failed.")
    {
        ValidationErrors = validationErrors;
    }
}