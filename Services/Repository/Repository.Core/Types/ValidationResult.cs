namespace Repository.Core.Types;

public record ValidationResult(bool IsValid, IEnumerable<string>? ValidationErrors = null);