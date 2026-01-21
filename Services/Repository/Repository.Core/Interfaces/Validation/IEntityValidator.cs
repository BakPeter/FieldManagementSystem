using Repository.Core.Types;

namespace Repository.Core.Interfaces.Validation;

public interface IEntityValidator<TEntity> where TEntity : class
{
    ValidationResult Validate(TEntity entity);
}