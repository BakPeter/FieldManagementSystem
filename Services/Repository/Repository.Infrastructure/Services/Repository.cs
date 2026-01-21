using Repository.Core.Interfaces;
using Repository.Core.Interfaces.Validation;
using Repository.Core.Types;

namespace Repository.Infrastructure.Services;

public class Repository<TEntity>(IRepositoryAdapter<TEntity> adapter, IEntityValidation<TEntity>? validation = null)
    : IRepository<TEntity>
    where TEntity : class
{
    private readonly IRepositoryAdapter<TEntity> _adapter = adapter;
    private readonly IEntityValidation<TEntity>? _validation = validation;

    public Task<Result<TEntity>> GetByIdAsync(int id) => _adapter.GetByIdAsync(id);

    public Task<Result<IEnumerable<TEntity>>> GetAllAsync() => _adapter.GetAllAsync();

    public async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        var validationResult = _validation?.Validate(entity);
        if (validationResult?.IsValid is false)
            return new Result<TEntity>(false, entity,
                new InValidEntityException(validationResult.ValidationErrors!));

        if (await _adapter.Contains(entity))
            return new Result<TEntity>(IsSuccess: false, entity, new EntityExistsException());

        return await _adapter.AddAsync(entity);
        // return new Result<TEntity>(true, entity);
    }

    public async Task<Result<TEntity>> UpdateAsync(TEntity entity)
    {
        if (await _adapter.Contains(entity) is false)
            return new Result<TEntity>(IsSuccess: false, entity, new EntityDontExistsException());
        return await _adapter.UpdateAsync(entity);
    }

    public async Task<Result<TEntity>> UpdateFieldAsync(int entityId, Func<TEntity, TEntity> updateAction)
    {
        var response = await _adapter.GetByIdAsync(entityId);
        if (response.IsSuccess is false) return new Result<TEntity>(IsSuccess: false, Error: new EntityDontExistsException());

        var newEntity = updateAction(response.Entity!);
        return await _adapter.AddAsync(newEntity);
    }

    public async Task<Result<TEntity>> DeleteAsync(TEntity entity)
    {
        if (await _adapter.Contains(entity) is false)
            return new Result<TEntity>(IsSuccess: false, entity, new EntityDontExistsException());
        return await _adapter.DeleteAsync(entity);
    }
}