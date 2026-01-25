using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Core.Types.DTOs;

namespace FieldManagementSystem.Services.Field.Core.Interfaces;

public interface IFieldService
{
    Task<Result<IEnumerable<FieldEntity>>> GetFieldsAsync();
    Task<Result<FieldEntity>> GetFieldAsync(string id);
    Task<Result<FieldEntity>> CreateFieldAsync(CreateFieldDto createFieldDto);
    Task<Result<string>> UpdateField(UpdateFieldDto updateFieldDto);
    Task<Result<string>> DeleteFieldAsync(string id);
}