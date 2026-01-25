namespace FieldManagementSystem.Services.Controller.Core.Types.DTOs;

public class UpdateControllerDto
{
    public Guid Id { get; set; }
    public ControllerStatus ControllerStatus { get; set; }
    public IEnumerable<string> AssiciatedFieldIds { get; set; }
    public IEnumerable<string> AuthorizedUserIds { get; set; }
}
