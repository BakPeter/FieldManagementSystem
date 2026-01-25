using FieldManagementSystem.Services.User.Core.Types;

namespace FieldManagementSystem.Services.User.Core.Interfaces.Validation;

public interface IUserValidator
{
    bool Validate(UserEntity entity, ref IEnumerable<string> validationErrors);
}