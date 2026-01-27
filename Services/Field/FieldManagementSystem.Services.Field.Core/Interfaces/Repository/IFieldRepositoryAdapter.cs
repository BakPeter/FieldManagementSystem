using FieldManagementSystem.Services.Field.Core.Types;

namespace FieldManagementSystem.Services.Field.Core.Interfaces.Repository;

public interface IFieldRepositoryAdapter
{
    Task<List<FieldEntity>> GetAllFieldsAsync(CancellationToken ct = default);
    Task<FieldEntity?> GetFieldAsync(string id, CancellationToken ct = default);
    Task<bool> CreateFieldAsync(FieldEntity fieldToAdd, CancellationToken ct = default);
    Task<bool> UpdateField(FieldEntity fieldToUpdate, CancellationToken ct = default);
    Task<bool> DeleteField(string id, CancellationToken ct = default);
    Task<FieldEntity?> GetFieldByNameAsync(string name, CancellationToken ct = default);
}