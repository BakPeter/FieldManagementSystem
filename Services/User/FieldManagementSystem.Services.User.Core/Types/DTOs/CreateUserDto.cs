namespace FieldManagementSystem.Services.User.Core.Types.DTOs;

public class CreateUserDto
{
    public string Email { get; set; }
    public UserType UserType { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
