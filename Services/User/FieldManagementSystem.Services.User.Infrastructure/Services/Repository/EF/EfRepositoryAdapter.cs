using FieldManagementSystem.Services.User.Core.Interfaces.Repository;
using FieldManagementSystem.Services.User.Core.Types;
using FieldManagementSystem.Services.User.Infrastructure.Configurations.Repository.EF;
using Microsoft.EntityFrameworkCore;

namespace FieldManagementSystem.Services.User.Infrastructure.Services.Repository.EF;

public class EfRepositoryAdapter : IUserRepositoryAdapter
{

    private readonly AppDbContext _db;

    public EfRepositoryAdapter(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<UserEntity>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.CreatedDate)
            .ToListAsync(ct);
        return users;
    }

    public Task<UserEntity?> GetUserAsync(string id, CancellationToken ct = default)
    {
        return _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.ToString().Equals(id), ct);
    }

    public Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        return _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.Equals(email), ct);
    }

    public async Task<UserEntity?> CreateUserAsync(UserEntity userToAdd, CancellationToken ct = default)
    {
        // userToAdd.CreatedDate = DateTime.UtcNow;
        // userToAdd.ModifiedDate = DateTime.UtcNow;

        _db.Users.Add(userToAdd);

        await _db.SaveChangesAsync(ct);

        return userToAdd;
    }

    public async Task<bool> UpdateUserAsync(UserEntity userToUpdate, CancellationToken ct = default)
    {
        var exists = await _db.Users.AnyAsync(x => x.Id == userToUpdate.Id, ct);

        if (exists is false) return false;

        userToUpdate.ModifiedDate = DateTime.UtcNow;

        _db.Users.Update(userToUpdate);

        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteUserAsync(string id, CancellationToken ct = default)
    {
        var entity = await _db.Users
            .FirstOrDefaultAsync(x => x.Id.ToString().Equals(id), ct);

        if (entity is null)
            return false;

        _db.Users.Remove(entity);

        await _db.SaveChangesAsync(ct);

        return true;
    }
}