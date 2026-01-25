using FieldManagementSystem.Services.Controller.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Controller.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Controller.Infrastructure.Services.Repository;

public class ControllerRepository : IControllerRepository
{
    private readonly ILogger<ControllerRepository> _logger;
    private readonly IControllerRepositoryAdapter _adapter;

    public ControllerRepository(ILogger<ControllerRepository> logger, IControllerRepositoryAdapter adapter)
    {
        _logger = logger;
        _adapter = adapter;
    }

    public Task<IEnumerable<ControllerEntity>> GetAllControllersAsync() => _adapter.GetAllControllersAsync();
    public Task<ControllerEntity?> GetControllerByNameAsync(string name) => _adapter.GetControllerByNameAsync(name);
    public Task<ControllerEntity?> GetControllerAsync(string id) => _adapter.GetControllerAsync(id);

    public Task<bool> CreateControllerAsync(ControllerEntity controllerToAdd) => _adapter.CreateControllerAsync(controllerToAdd);
    public Task<bool> UpdateController(ControllerEntity controllerToUpdate) => _adapter.UpdateController(controllerToUpdate);
    public Task<bool> DeleteController(string id) => _adapter.DeleteController(id);
}