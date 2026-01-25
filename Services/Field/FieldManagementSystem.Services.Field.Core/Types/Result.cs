namespace FieldManagementSystem.Services.Field.Core.Types;

public record Result<TData>(bool IsSuccess, TData? Data = null, Exception? Error = null) where TData: class?;