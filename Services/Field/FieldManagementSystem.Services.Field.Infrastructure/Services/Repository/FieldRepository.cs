using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Field.Infrastructure.Services.Repository;

public class FieldRepository : IFieldRepository
{
    private readonly ILogger<FieldRepository> _logger;
    private readonly IFieldRepositoryAdapter _adapter;

    public FieldRepository(ILogger<FieldRepository> logger, IFieldRepositoryAdapter adapter)
    {
        _logger = logger;
        _adapter = adapter;
    }

    public Task<List<FieldEntity>> GetAllFieldsAsync(CancellationToken ct) => _adapter.GetAllFieldsAsync(ct);
    public Task<FieldEntity?> GetFieldByNameAsync(string name) => _adapter.GetFieldByNameAsync(name);
    public Task<FieldEntity?> GetFieldAsync(string id, CancellationToken ct = default) => _adapter.GetFieldAsync(id, ct);
    public Task<bool> CreateFieldAsync(FieldEntity fieldToAdd) => _adapter.CreateFieldAsync(fieldToAdd);
    public Task<bool> UpdateField(FieldEntity fieldToUpdate) => _adapter.UpdateField(fieldToUpdate);
    public Task<bool> DeleteField(string id) => _adapter.DeleteField(id);
}