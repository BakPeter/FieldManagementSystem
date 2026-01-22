namespace FieldManagementSystem.Services.Repository.Core.Types;

public record ValidationResult(bool IsValid, IEnumerable<string>? ValidationErrors = null);