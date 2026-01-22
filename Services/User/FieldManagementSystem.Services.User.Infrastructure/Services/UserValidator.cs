using Repository.Core.Interfaces.Validation;
using Repository.Core.Types;

namespace FieldManagementSystem.User.Infrastructure.Services;

public class UserValidator: IEntityValidator<Core.Types.User>
{
    public ValidationResult Validate(Core.Types.User entity)
    {
        return new ValidationResult(true);
    }
}