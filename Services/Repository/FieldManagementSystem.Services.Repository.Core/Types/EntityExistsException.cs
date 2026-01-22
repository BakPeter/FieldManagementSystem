namespace FieldManagementSystem.Services.Repository.Core.Types;

public class EntityExistsException : Exception
{
    public EntityExistsException() { }
    public EntityExistsException(string msg) : base(msg) { }
    public EntityExistsException(string msg, Exception innerException) : base(msg, innerException) { }
}