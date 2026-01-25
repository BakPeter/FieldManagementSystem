namespace FieldManagementSystem.Services.Controller.Core.Types;

public class ControllerEntity
{
    public Guid Id { get; set; }
    public ControllerType ControllerType { get; set; }
    public string Name { get; set; }
    public ControllerStatus ControllerStatus { get; set; }
    public IEnumerable<string> AssiciatedFieldIds { get; set; }
    public IEnumerable<string> AuthorizedUserIds{ get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}