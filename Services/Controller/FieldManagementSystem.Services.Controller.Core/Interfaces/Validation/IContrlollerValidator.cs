using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;

public interface IContrlollerValidator
{
    bool Validate(ControllerEntity entity, ref IEnumerable<string> validationErrors);
}