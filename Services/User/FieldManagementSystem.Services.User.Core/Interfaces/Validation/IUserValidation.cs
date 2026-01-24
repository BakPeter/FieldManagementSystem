using FieldManagementSystem.User.Core.Types;

namespace FieldManagementSystem.User.Core.Interfaces.Validation;

public interface IUserValidation
{
    bool Validate(UserEntity entity, out IEnumerable<string> validationErrors);

}