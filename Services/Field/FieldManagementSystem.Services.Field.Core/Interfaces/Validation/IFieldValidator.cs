using FieldManagementSystem.Services.Field.Core.Types;

namespace FieldManagementSystem.Services.Field.Core.Interfaces.Validation;

public interface IFieldValidator
{
    bool Validate(FieldEntity entity, ref IEnumerable<string> validationErrors);
}