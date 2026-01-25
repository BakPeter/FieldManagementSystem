using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Core.Interfaces.Repository;

public interface IControllerRepository
{
    Task<IEnumerable<ControllerEntity>> GetAllControllersAsync();
    Task<ControllerEntity?> GetControllerAsync(string token);
    Task<ControllerEntity?> GetControllerByNameAsync(string name);
    Task<bool> CreateControllerAsync(ControllerEntity controllerToAdd);
    Task<bool> UpdateController(ControllerEntity controllerToUpdate);
    Task<bool> DeleteController(string id);
}