using FieldManagementSystem.Services.Field.Core.Types;

namespace FieldManagementSystem.Services.Field.Core.Interfaces.Validation;

public interface IFieldValidation
{
    bool Validate(FieldEntity entity, out IEnumerable<string> validationErrors);

}