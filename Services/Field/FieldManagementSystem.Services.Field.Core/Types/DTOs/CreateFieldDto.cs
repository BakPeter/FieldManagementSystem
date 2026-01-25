namespace FieldManagementSystem.Services.Field.Core.Types.DTOs;

public class CreateFieldDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> AuthorizedUsers { get; set; }
}
