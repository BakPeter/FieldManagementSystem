using FieldManagementSystem.Services.Field.Core.Types;

namespace FieldManagementSystem.Services.Field.Core.Interfaces.Repository;

public interface IFieldRepository
{
    Task<IEnumerable<FieldEntity>> GetAllFieldsAsync();
    Task<FieldEntity?> GetFieldAsync(string id);
    Task<bool> CreateFieldAsync(FieldEntity fieldToAdd);
    Task<bool> UpdateField(FieldEntity fieldToUpdate);
    Task<bool> DeleteField(string id);
    Task<FieldEntity?> GetFieldByNameAsync(string name);
}