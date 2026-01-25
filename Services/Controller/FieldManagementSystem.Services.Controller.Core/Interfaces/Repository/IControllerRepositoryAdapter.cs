using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Core.Interfaces.Repository;

public interface IControllerRepositoryAdapter
{
    Task<IEnumerable<ControllerEntity>> GetAllControllersAsync();
    Task<ControllerEntity?> GetControllerAsync(string id);
    Task<ControllerEntity?> GetControllerByNameAsync(string name);
    Task<bool> CreateControllerAsync(ControllerEntity controllerToAdd);
    Task<bool> UpdateController(ControllerEntity controllerToUpdate);
    Task<bool> DeleteController(string id);
}