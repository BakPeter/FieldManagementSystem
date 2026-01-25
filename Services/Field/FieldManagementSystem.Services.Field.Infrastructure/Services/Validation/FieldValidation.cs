using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Field.Infrastructure.Services.Validation;

public class FieldValidation: IFieldValidation
{
    private readonly ILogger<FieldValidation> _logger;
    private readonly IEnumerable<IFieldValidator> _validators;

    public FieldValidation(ILogger<FieldValidation> logger, IEnumerable<IFieldValidator> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public bool Validate(FieldEntity entity, out IEnumerable<string> validationErrors)
    {
        validationErrors  = new List<string>();

        foreach (var validator in _validators)
            _ = validator.Validate(entity, ref validationErrors);

        return !validationErrors.Any();
    }
}