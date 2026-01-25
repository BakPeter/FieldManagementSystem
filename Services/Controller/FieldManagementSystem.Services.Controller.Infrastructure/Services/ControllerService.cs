using System.Text.Json;
using FieldManagementSystem.Services.Controller.Core.Interfaces;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Controller.Core.Types;
using FieldManagementSystem.Services.Controller.Core.Types.DTOs;
using FieldManagementSystem.Services.Controller.Infrastructure.types;
using Microsoft.Extensions.Logging;


namespace FieldManagementSystem.Services.Controller.Infrastructure.Services;

public class ControllerService : IControllerService
{
    private readonly ILogger<ControllerService> _logger;
    private readonly IControllerRepository _repository;
    private readonly IControllerValidation _validation;

    public ControllerService(ILogger<ControllerService> logger,
        IControllerRepository repository,
        IControllerValidation validation)
    {
        _logger = logger;
        _repository = repository;
        _validation = validation;
    }

    public async Task<Result<IEnumerable<ControllerEntity>>> GetControllersAsync()
    {
        var users = (await _repository.GetAllControllersAsync()).ToList();
        var result = new Result<IEnumerable<ControllerEntity>>(true, users);
        _logger.LogInformation("Get Users - Success: {Success} Result: {result}", result.IsSuccess, JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<Result<ControllerEntity>> GetControllerAsync(string id)
    {
        var user = await _repository.GetControllerAsync(id);

        if (user == null)
        {
            var errorResult = new Result<ControllerEntity>(false, null,
                new ArgumentException($"User with token {id} not found", nameof(id)));

            _logger.LogInformation("Get User - token: {token} Success: {Success} Error: {Error}",
                id, errorResult.IsSuccess, errorResult.Error!.Message);

            return errorResult;
        }

        var result = new Result<ControllerEntity>(true, user);
        _logger.LogInformation("Get User - token: {token} Success: {Success} Result: {result}",
            id, result.IsSuccess, JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<Result<ControllerEntity>> CreateControllerAsync(CreateControllerDto createControllerDto)
    {
        var currentTime = DateTime.UtcNow;
        var controllerToAdd = new ControllerEntity()
        {
            Id = Guid.NewGuid(),
            ControllerType = createControllerDto.ControllerType,
            Name = createControllerDto.Name,
            ControllerStatus = ControllerStatus.Inactive,
            AssiciatedFieldIds = createControllerDto.AssiciatedFieldIds,
            AuthorizedUserIds = createControllerDto.AuthorizedUserIds,
            CreatedDate = currentTime,
            ModifiedDate = currentTime
        };

        if (_validation.Validate(controllerToAdd, out var validationErrors) is false)
            return new Result<ControllerEntity>(false, null, new ControllerValidationException(validationErrors));

        var controller = await _repository.GetControllerByNameAsync(controllerToAdd.Name);
        if (controller is not null)
            return new Result<ControllerEntity>(false, null,
                new ArgumentException($"Controller named {controllerToAdd.Name} exists", nameof(createControllerDto)));

        var isUserAdded = await _repository.CreateControllerAsync(controllerToAdd);
        return isUserAdded ? new Result<ControllerEntity>(true, controllerToAdd) : new Result<ControllerEntity>(false, null, new Exception("Failed to add user to repository."));
    }

    public async Task<Result<string>> UpdateController(UpdateControllerDto updateControllerDto)
    {
        var getControllerResult = await GetControllerAsync(updateControllerDto.Id.ToString());
        if (getControllerResult.IsSuccess is false)
            return new Result<string>(false, null,
                new ArgumentException($"Controller with id {updateControllerDto.Id.ToString()} not found", nameof(updateControllerDto)));

        var controller = getControllerResult.Data!;
        var updateController = new ControllerEntity()
        {
            Id = controller.Id,
            ControllerType = controller.ControllerType,
            Name = controller.Name,
            ControllerStatus = updateControllerDto.ControllerStatus,
            AssiciatedFieldIds = updateControllerDto.AssiciatedFieldIds,
            AuthorizedUserIds = updateControllerDto.AuthorizedUserIds,
            CreatedDate = controller.CreatedDate,
            ModifiedDate = DateTime.UtcNow
        };

        if (_validation.Validate(updateController, out var validationErrors) is false)
            return new Result<string>(false, "User Validation Failed", new ControllerValidationException(validationErrors));

        var isControllerUpdated = await _repository.UpdateController(updateController);
        return isControllerUpdated
            ? new Result<string>(true, $"Controller with id {updateController.Id} updated")
            : new Result<string>(false, null, new Exception($"Controller with id  {updateController.Id} update failed"));
    }
    
    public async Task<Result<string>> DeleteControllerAsync(string id)
    {
        var getControllerResponse = await GetControllerAsync(id);
        if (getControllerResponse.IsSuccess is false)
            return new Result<string>(false, null,
                new ArgumentException($"controller with id {id} not found", nameof(id)));
    
        var isControllerDeleted = await _repository.DeleteController(id);
        return isControllerDeleted
                ? new Result<string>(isControllerDeleted, $"User {id} deleted")
            : new Result<string>(false, null, new Exception($"User id {id} delete failed"));
    }
}