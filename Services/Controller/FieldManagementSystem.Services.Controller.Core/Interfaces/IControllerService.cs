using FieldManagementSystem.Services.Controller.Core.Types;
using FieldManagementSystem.Services.Controller.Core.Types.DTOs;

namespace FieldManagementSystem.Services.Controller.Core.Interfaces;

public interface IControllerService
{
    Task<Result<IEnumerable<ControllerEntity>>> GetControllersAsync();
    Task<Result<ControllerEntity>> GetControllerAsync(string id);
    Task<Result<ControllerEntity>> CreateControllerAsync(CreateControllerDto createControllerDto);
    Task<Result<string>> UpdateController(UpdateControllerDto updateControllerDto);
    Task<Result<string>> DeleteControllerAsync(string id);
}