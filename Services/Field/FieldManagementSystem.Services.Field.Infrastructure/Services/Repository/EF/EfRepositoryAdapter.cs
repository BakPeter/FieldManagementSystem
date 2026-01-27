using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.Field.Infrastructure.Services.Repository.EF;

public class EfRepositoryAdapter : IFieldRepositoryAdapter
{
    private readonly ILogger<EfRepositoryAdapter> _logger;

    private readonly AppDbContext _db;

    public EfRepositoryAdapter(ILogger<EfRepositoryAdapter> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public Task<List<FieldEntity>> GetAllFieldsAsync(CancellationToken ct = default)
    {
        return _db.Fields
            .AsNoTracking()
            .Include(f => f.AuthorizedUserLinks)
            .OrderBy(f => f.CreatedDate)
            .ToListAsync(ct);
    }

    public Task<FieldEntity?> GetFieldAsync(string id, CancellationToken ct = default)
    {
        // var fields = await _db.Fields
        //     .AsNoTracking()
        //     .Include(f => f.AuthorizedUserLinks)
        //     .ToListAsync(cancellationToken: ct);

        return _db.Fields
            .AsNoTracking()
            .Include(f => f.AuthorizedUserLinks)
            .FirstOrDefaultAsync(f => f.Id.ToString().Equals(id), ct);
    }

    public async Task<bool> CreateFieldAsync(FieldEntity fieldToAdd, CancellationToken ct = default)
    {
        // entity.CreatedDate = DateTime.UtcNow;
        // entity.ModifiedDate = DateTime.UtcNow;

        // foreach (var link in fieldToAdd.AuthorizedUserLinks)
        //     link.FieldId = fieldToAdd.Id; 
        //
        try
        {
            _db.Fields.Add(fieldToAdd);
            await _db.SaveChangesAsync(ct);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }


        // if (fieldToAdd.AuthorizedUserLinks.Count > 0)
        // {
        //     _db.FieldAuthorizedUsers.AddRange(fieldToAdd.AuthorizedUserLinks);
        //     await _db.SaveChangesAsync(ct);
        // }

        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateField(FieldEntity fieldToUpdate, CancellationToken ct = default)
    {
        var existing = await _db.Fields
            .Include(f => f.AuthorizedUserLinks)
            .FirstOrDefaultAsync(f => f.Id == fieldToUpdate.Id, ct);

        if (existing is null) return false;

        existing.Name = fieldToUpdate.Name;
        existing.Description = fieldToUpdate.Description;
        existing.ModifiedDate = DateTime.UtcNow;

        // Sync authorized users
        var desiredUserIds = fieldToUpdate.AuthorizedUserLinks.Select(x => x.UserId).ToHashSet();
        var currentUserIds = existing.AuthorizedUserLinks.Select(x => x.UserId).ToHashSet();

        // remove
        var toRemove = existing.AuthorizedUserLinks.Where(x => !desiredUserIds.Contains(x.UserId)).ToList();
        if (toRemove.Count > 0)
            _db.FieldAuthorizedUsers.RemoveRange(toRemove);

        // add
        var toAddIds = desiredUserIds.Except(currentUserIds).ToList();
        foreach (var userId in toAddIds)
        {
            existing.AuthorizedUserLinks.Add(new FieldAuthorizedUserRow
            {
                FieldId = existing.Id,
                UserId = userId
            });
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteField(string id, CancellationToken ct = default)
    {
        var existing = await _db.Fields.FirstOrDefaultAsync(f => f.Id.ToString().Equals(id), ct);
        if (existing is null) return false;

        _db.Fields.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public Task<FieldEntity?> GetFieldByNameAsync(string name, CancellationToken ct = default)
    {
        return _db.Fields
        .AsNoTracking()
        .Include(f => f.AuthorizedUserLinks)
        .FirstOrDefaultAsync(f => f.Name.Equals(name), ct);
    }
}