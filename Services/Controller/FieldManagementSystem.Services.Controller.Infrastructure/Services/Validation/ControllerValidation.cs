using FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Controller.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Controller.Infrastructure.Services.Validation;

public class ControllerValidation: IControllerValidation
{
    private readonly ILogger<ControllerValidation> _logger;
    private readonly IEnumerable<IContrlollerValidator> _validators;

    public ControllerValidation(ILogger<ControllerValidation> logger, IEnumerable<IContrlollerValidator> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public bool Validate(ControllerEntity entity, out IEnumerable<string> validationErrors)
    {
        validationErrors  = new List<string>();

        foreach (var validator in _validators)
            _ = validator.Validate(entity, ref validationErrors);

        return !validationErrors.Any();
    }
}