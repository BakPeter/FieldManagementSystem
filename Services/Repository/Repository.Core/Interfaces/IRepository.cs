using Repository.Core.Types;

namespace Repository.Core.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<Result<TEntity>> GetByIdAsync(int id);
    Task<Result<IEnumerable<TEntity>>> GetAllAsync();
    Task<Result<TEntity>> AddAsync(TEntity entity);
    Task<Result<TEntity>> UpdateAsync(TEntity entity);
    Task<Result<TEntity>> UpdateFieldAsync(int entityId, Func<TEntity, TEntity> updateAction);
    Task<Result<TEntity>> DeleteAsync(TEntity entity);
}