using FieldManagementSystem.Services.Field.Core.Types;

namespace FieldManagementSystem.Services.Field.Core.Interfaces.Repository;

public interface IFieldRepository
{
    Task<List<FieldEntity>> GetAllFieldsAsync(CancellationToken ct = default);
    Task<FieldEntity?> GetFieldAsync(string id, CancellationToken ct = default);
    Task<bool> CreateFieldAsync(FieldEntity fieldToAdd);
    Task<bool> UpdateField(FieldEntity fieldToUpdate);
    Task<bool> DeleteField(string id);
    Task<FieldEntity?> GetFieldByNameAsync(string name);
}