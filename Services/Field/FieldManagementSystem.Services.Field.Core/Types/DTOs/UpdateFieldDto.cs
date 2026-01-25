namespace FieldManagementSystem.Services.Field.Core.Types.DTOs;

public class UpdateFieldDto
{
    public string Id { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> AuthorizedUsers { get; set; }
}
