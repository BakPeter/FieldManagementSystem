using FieldManagementSystem.Services.Repository.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Repository.Core.Types;

namespace FieldManagementSystem.Services.Repository.Infrastructure.Services;

public class EntityValidation<TEntity> : IEntityValidation<TEntity> where TEntity : class
{
    private readonly IEnumerable<IEntityValidator<TEntity>> _validators;

    public EntityValidation(IEnumerable<IEntityValidator<TEntity>> validators)
    {
        _validators = validators;
    }

    public ValidationResult Validate(TEntity entity)
    {
        if (_validators.Any() is false) return new ValidationResult(true);

        var validationsErrors = new List<string>();
        foreach (var validator in _validators)
        {
            var currResult = validator.Validate(entity);
            if (currResult.IsValid) continue;

            validationsErrors.AddRange(currResult.ValidationErrors!);
        }

        return validationsErrors.Count > 0
            ? new ValidationResult(false, validationsErrors)
            : new ValidationResult(true);
    }
}