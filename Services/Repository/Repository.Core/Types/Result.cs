namespace Repository.Core.Types;

public record Result<T>(bool IsSuccess, T? Entity = null, Exception? Error = null) where T : class;