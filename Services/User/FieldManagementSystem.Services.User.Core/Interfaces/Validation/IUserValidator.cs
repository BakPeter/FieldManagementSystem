using FieldManagementSystem.User.Core.Types;

namespace FieldManagementSystem.User.Core.Interfaces.Validation;

public interface IUserValidator
{
    bool Validate(UserEntity entity, ref IEnumerable<string> validationErrors);
}