namespace FieldManagementSystem.Services.Repository.Core.Types;

public class EntityDontExistsException : Exception
{
    public EntityDontExistsException() { }
    public EntityDontExistsException(string msg) : base(msg) { }
    public EntityDontExistsException(string msg, Exception innerException) : base(msg, innerException) { }
}