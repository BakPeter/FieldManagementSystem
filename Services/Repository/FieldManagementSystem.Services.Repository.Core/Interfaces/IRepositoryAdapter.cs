using FieldManagementSystem.Services.Repository.Core.Types;

namespace FieldManagementSystem.Services.Repository.Core.Interfaces;

public interface IRepositoryAdapter<TEntity> where TEntity : class
{
    Task<Result<TEntity>> GetByIdAsync(int id);
    Task<Result<IEnumerable<TEntity>>> GetAllAsync();
    Task<Result<TEntity>> AddAsync(TEntity entity);
    Task<Result<TEntity>> UpdateAsync(TEntity entity);
    Task<Result<TEntity>> DeleteAsync(TEntity entity);
    Task<bool> Contains(TEntity entity);
}