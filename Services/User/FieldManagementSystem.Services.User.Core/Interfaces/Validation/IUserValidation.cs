using FieldManagementSystem.Services.User.Core.Types;

namespace FieldManagementSystem.Services.User.Core.Interfaces.Validation;

public interface IUserValidation
{
    bool Validate(UserEntity entity, out IEnumerable<string> validationErrors);

}