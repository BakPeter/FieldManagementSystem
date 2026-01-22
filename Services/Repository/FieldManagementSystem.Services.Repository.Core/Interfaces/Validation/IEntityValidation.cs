using FieldManagementSystem.Services.Repository.Core.Types;

namespace FieldManagementSystem.Services.Repository.Core.Interfaces.Validation;

public interface IEntityValidation<TEntity> where TEntity : class
{
    ValidationResult Validate(TEntity entity);
}