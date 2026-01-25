namespace FieldManagementSystem.Services.Controller.Core.Types.DTOs;

public class CreateControllerDto
{
    public string Name { get; set; }
    public ControllerType ControllerType { get; set; }
    public IEnumerable<string> AssiciatedFieldIds { get; set; }
    public IEnumerable<string> AuthorizedUserIds { get; set; }
}
