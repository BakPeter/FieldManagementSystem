using FieldManagementSystem.Services.Repository.Core.Types;

namespace FieldManagementSystem.Services.Repository.Core.Interfaces.Validation;

public interface IEntityValidator<TEntity> where TEntity : class
{
    ValidationResult Validate(TEntity entity);
}