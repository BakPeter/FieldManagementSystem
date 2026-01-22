using Repository.Core.Interfaces;
using Repository.Core.Types;

namespace FieldManagementSystem.User.Infrastructure.Services;

public class UserCachedRepositoryAdapter : IRepositoryAdapter<Core.Types.User>
{
    private static readonly List<User> _users = new();

    public Task<Result<Core.Types.User>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Core.Types.User>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Core.Types.User>> AddAsync(Core.Types.User entity)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Core.Types.User>> UpdateAsync(Core.Types.User entity)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Core.Types.User>> DeleteAsync(Core.Types.User entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Contains(Core.Types.User entity)
    {
        throw new NotImplementedException();
    }
}