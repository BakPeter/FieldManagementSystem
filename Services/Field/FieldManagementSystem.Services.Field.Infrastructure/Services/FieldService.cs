using System.Text.Json;
using FieldManagementSystem.Services.Field.Core.Interfaces;
using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Core.Types.DTOs;
using FieldManagementSystem.Services.Field.Infrastructure.types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Field.Infrastructure.Services;

public class FieldService : IFieldService
{
    private readonly ILogger<FieldService> _logger;
    private readonly IFieldRepository _repository;
    private readonly IFieldValidation _validation;

    public FieldService(ILogger<FieldService> logger, IFieldRepository repository, IFieldValidation validation)
    {
        _logger = logger;
        _repository = repository;
        _validation = validation;
    }

    public async Task<Result<IEnumerable<FieldEntity>>> GetFieldsAsync(CancellationToken ct = default)
    {
        var fields = (await _repository.GetAllFieldsAsync(ct)).ToList();
        var result = new Result<IEnumerable<FieldEntity>>(true, fields);
        _logger.LogInformation("Get Fields- Success: {Success} Result: {result}", result.IsSuccess, JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<Result<FieldEntity>> GetFieldAsync(string id, CancellationToken ct)
    {
        var field = await _repository.GetFieldAsync(id, ct);

        if (field == null)
        {
            var errorResult = new Result<FieldEntity>(false, null,
                new ArgumentException($"Field with id {id} not found", nameof(id)));

            _logger.LogInformation("Get Field - id: {id} Success: {Success} Error: {Error}",
                id, errorResult.IsSuccess, errorResult.Error!.Message);

            return errorResult;
        }

        var result = new Result<FieldEntity>(true, field);
        _logger.LogInformation("Get Field - id: {id} Success: {Success} Result: {result}",
            id, result.IsSuccess, JsonSerializer.Serialize(result));
        return result;

    }

    public async Task<Result<FieldEntity>> CreateFieldAsync(CreateFieldDto createFieldDto, CancellationToken ct = default)
    {
        var currentTime = DateTime.UtcNow;
        var fieldToAdd = new FieldEntity()
        {
            Id = Guid.NewGuid(),
            Name = createFieldDto.Name,
            Description = createFieldDto.Description,
            AuthorizedUsers = createFieldDto.AuthorizedUsers,
            CreatedDate = currentTime,
            ModifiedDate = currentTime
        };

        if (_validation.Validate(fieldToAdd, out var validationErrors) is false)
            return new Result<FieldEntity>(false, null, new FieldValidationException(validationErrors));

        var field = await _repository.GetFieldByNameAsync(fieldToAdd.Name);
        if (field is not null)
            return new Result<FieldEntity>(false, null,
                new ArgumentException($"Field with the name {createFieldDto.Name} exists", nameof(createFieldDto)));

        var isFieldAdded = await _repository.CreateFieldAsync(fieldToAdd);
        return isFieldAdded ? new Result<FieldEntity>(true, fieldToAdd) : new Result<FieldEntity>(false, null, new Exception("Failed to add Field to repository."));
    }

    public async Task<Result<string>> UpdateField(UpdateFieldDto updateFieldDto, CancellationToken ct = default)
    {
        var getFieldResponse = await GetFieldAsync(updateFieldDto.Id, CancellationToken.None);
        if (getFieldResponse.IsSuccess is false)
            return new Result<string>(false, null,
                new ArgumentException($"Field with id {updateFieldDto.Id} not found", nameof(updateFieldDto)));

        var field  = getFieldResponse.Data!;
        var updatedField = new FieldEntity()
        {
            Id = field .Id,
            Name = field.Name,
            AuthorizedUsers = updateFieldDto.AuthorizedUsers,
            Description = updateFieldDto.Description,
            CreatedDate = field .CreatedDate,
            ModifiedDate = DateTime.UtcNow
        };

        if (_validation.Validate(updatedField, out var validationErrors) is false)
            return new Result<string>(false, "Field Validation Failed", new FieldValidationException(validationErrors));

        var isFieldUpdated = await _repository.UpdateField(updatedField);
        return isFieldUpdated
            ? new Result<string>(true, $"Field with id {updatedField.Id} updated")
            : new Result<string>(false, null, new Exception($"Field {updatedField.Id} update failed"));
    }

    public async Task<Result<string>> DeleteFieldAsync(string id, CancellationToken ct = default)
    {
        var getFieldResponse = await GetFieldAsync(id, CancellationToken.None);
        if (getFieldResponse.IsSuccess is false)
            return new Result<string>(false, null,
                new ArgumentException($"Field with id {id} not found", nameof(id)));

        var isFieldDeleted = await _repository.DeleteField(id);
        return isFieldDeleted
                ? new Result<string>(isFieldDeleted, $"Field {id} deleted")
            : new Result<string>(false, null, new Exception($"Field with id {id} delete failed"));
    }
}