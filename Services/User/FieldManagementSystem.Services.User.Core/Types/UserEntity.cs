namespace FieldManagementSystem.Services.User.Core.Types;

public class UserEntity
{
    public Guid Id { get; set; }
    public int UserTypeId { get; set; }

    // Convenient enum for your code (NOT mapped to DB)
    public UserType UserType
    {
        get => (UserType)UserTypeId;
        set => UserTypeId = (int)value;
    }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}