using FieldManagementSystem.Services.User.Core.Interfaces.Validation;
using FieldManagementSystem.Services.User.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.User.Infrastructure.Services.Validation;

public class UserValidation: IUserValidation
{
    private readonly ILogger<UserValidation> _logger;
    private readonly IEnumerable<IUserValidator> _validators;

    public UserValidation(ILogger<UserValidation> logger, IEnumerable<IUserValidator> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public bool Validate(UserEntity entity, out IEnumerable<string> validationErrors)
    {
        validationErrors  = new List<string>();

        foreach (var validator in _validators)
            _ = validator.Validate(entity, ref validationErrors);

        return !validationErrors.Any();
    }
}