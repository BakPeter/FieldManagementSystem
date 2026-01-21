using Repository.Core.Types;

namespace Repository.Core.Interfaces.Validation;

public interface IEntityValidation<TEntity> where TEntity : class
{
    ValidationResult Validate(TEntity entity);
}