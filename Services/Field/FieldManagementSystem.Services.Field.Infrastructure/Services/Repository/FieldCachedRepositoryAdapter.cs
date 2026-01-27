using System.Collections.Concurrent;
using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Field.Infrastructure.Services.Repository;

public class FieldCachedRepositoryAdapter : IFieldRepositoryAdapter
{
    private readonly ConcurrentDictionary<string, FieldEntity> _data = new();
    private readonly ILogger<FieldCachedRepositoryAdapter> _logger;

    public FieldCachedRepositoryAdapter(ILogger<FieldCachedRepositoryAdapter> logger)
    {
        var id1 = Guid.NewGuid();
        _data[id1.ToString()] = new FieldEntity
        {
            Id = id1,
            Name = "Green Field",
            Description = "Field Of Cucumbers",
            AuthorizedUsers = new List<string> { "user1@gmail.com", "user2@gmail.com" },
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        var id2 = Guid.NewGuid();
        _data[id2.ToString()] = new FieldEntity
        {
            Id = id2,
            Name = "Orange Field",
            Description = "Oranges Field",
            AuthorizedUsers = new List<string> { "user1@gmail.com" },
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _logger = logger;
    }

    public Task<List<FieldEntity>> GetAllFieldsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_data.Values.ToList());
    }

    public Task<FieldEntity?> GetFieldAsync(string id, CancellationToken ct = default)
    {
        _ = _data.TryGetValue(id, out var field);
        return Task.FromResult(field);
    }

    public Task<bool> CreateFieldAsync(FieldEntity fieldToAdd, CancellationToken ct = default)
    {
        return Task.FromResult(_data.TryAdd(fieldToAdd.Id.ToString(), fieldToAdd));
    }

    public Task<bool> UpdateField(FieldEntity fieldToUpdate, CancellationToken ct = default)
    {
        try
        {
            _data[fieldToUpdate.Id.ToString()] = fieldToUpdate;
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            _logger.LogError("Failed To Update User, {userToUpdate}", fieldToUpdate);
            throw;
        }
    }

    public Task<bool> DeleteField(string id, CancellationToken ct = default)
    {
        return Task.FromResult(_data.TryRemove(id, out _));
    }

    public Task<FieldEntity?> GetFieldByNameAsync(string name, CancellationToken ct = default)
    {
        var field = _data.Values.FirstOrDefault(f => f.Name.Equals(name));
        return Task.FromResult(field);
    }
}