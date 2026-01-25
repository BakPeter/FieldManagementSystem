namespace FieldManagementSystem.Services.User.Core.Types;

public class UserEntity
{
    public Guid Id { get; set; }
    public UserType UserType { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}