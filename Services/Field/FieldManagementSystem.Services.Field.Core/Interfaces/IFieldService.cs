using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Core.Types.DTOs;

namespace FieldManagementSystem.Services.Field.Core.Interfaces;

public interface IFieldService
{
    Task<Result<IEnumerable<FieldEntity>>> GetFieldsAsync(CancellationToken ct = default);
    Task<Result<FieldEntity>> GetFieldAsync(string id, CancellationToken ct = default);
    Task<Result<FieldEntity>> CreateFieldAsync(CreateFieldDto createFieldDto, CancellationToken ct = default);
    Task<Result<string>> UpdateField(UpdateFieldDto updateFieldDto, CancellationToken ct = default);
    Task<Result<string>> DeleteFieldAsync(string id, CancellationToken ct = default);
}