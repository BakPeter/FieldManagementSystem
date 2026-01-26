using FieldManagementSystem.Services.User.Core.Interfaces.Repository;
using FieldManagementSystem.Services.User.Core.Types;
using FieldManagementSystem.Services.User.Infrastructure.Configurations.Repository.EF;
using Microsoft.EntityFrameworkCore;

namespace FieldManagementSystem.Services.User.Infrastructure.Services.Repository.EF;

public class EfRepositoryAdapter: IUserRepositoryAdapter
{

    private readonly AppDbContext _db;

    public EfRepositoryAdapter(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<UserEntity>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _db.Users.AsNoTracking().OrderBy(u => u.CreatedDate).ToListAsync(ct);
        return users;
    }

    public Task<UserEntity?> GetUserAsync(string email, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateUserAsync(UserEntity userToAdd, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUser(UserEntity userToUpdate, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(string id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}