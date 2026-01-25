using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;

public interface IControllerValidation
{
    bool Validate(ControllerEntity entity, out IEnumerable<string> validationErrors);

}