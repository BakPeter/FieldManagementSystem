using System.Text.Json;

namespace Repository.Core.Types;

public class InValidEntityException : Exception
{

    public InValidEntityException(IEnumerable<string> validationErrors) :
        base($"Validation Errors: {JsonSerializer.Serialize(validationErrors)}")
    {
    }

    public InValidEntityException(IEnumerable<string> validationErrors, Exception innerException) :
        base($"Validation Errors: {JsonSerializer.Serialize(validationErrors)}", innerException)
    {
    }
}