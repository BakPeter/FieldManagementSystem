using System.Collections.Concurrent;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Controller.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Controller.Infrastructure.Services.Repository;

public class ControllerCachedRepositoryAdapter : IControllerRepositoryAdapter
{
    private readonly ConcurrentDictionary<string, ControllerEntity> _data = new();
    private readonly ILogger<ControllerCachedRepositoryAdapter> _logger;

    public ControllerCachedRepositoryAdapter(ILogger<ControllerCachedRepositoryAdapter> logger)
    {
        var id1 = Guid.NewGuid();
        _data[id1.ToString()] = new ControllerEntity
        {
            Id = id1,
            ControllerType = ControllerType.Irrigation,
            Name = "Irrigation Controller 1",
            ControllerStatus = ControllerStatus.Inactive,
            AssiciatedFieldIds = new List<string> { "field-1-uuid", "field-2-uuid" },
            AuthorizedUserIds = new List<string> { "admin-user-1-uuid", "costumer-user-1-uuid", "costumer-user-2-uuid" },
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        var id2 = Guid.NewGuid();
        _data[id2.ToString()] = new ControllerEntity
        {
            Id = id2,
            ControllerType = ControllerType.Sensor,
            Name = "Heat Sensor",
            ControllerStatus = ControllerStatus.Active,
            AssiciatedFieldIds = new List<string> { "field-1-uuid", "field-3-uuid" },
            AuthorizedUserIds = new List<string> { "admin-user-1-uuid", "costumer-user-1-uuid" },
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _logger = logger;
    }

    public Task<IEnumerable<ControllerEntity>> GetAllControllersAsync()
    {
        return Task.FromResult(_data.Values.AsEnumerable());
    }

    public Task<ControllerEntity?> GetControllerAsync(string id)
    {
        _ = _data.TryGetValue(id, out var controller);
        return Task.FromResult(controller);
    }


    public Task<ControllerEntity?> GetControllerByNameAsync(string name)
    {
        var controller = _data.Values.FirstOrDefault(entity => entity.Name.Equals(name));
        return Task.FromResult(controller);
    }
    
    public Task<bool> CreateControllerAsync(ControllerEntity controllerToAdd)
    {
        return Task.FromResult(_data.TryAdd(controllerToAdd.Id.ToString(), controllerToAdd));
    }

    public Task<bool> UpdateController(ControllerEntity controllerToUpdate)
    {
        try
        {
            _data[controllerToUpdate.Id.ToString()] = controllerToUpdate;
            return Task.FromResult(true);
        }
        catch (Exception )
        {
            _logger.LogError("Failed To Update User, {userToUpdate}", controllerToUpdate);
            throw;
        }
    }
    
    public Task<bool> DeleteController(string id)
    {
        return Task.FromResult(_data.TryRemove(id, out _));
    }
}