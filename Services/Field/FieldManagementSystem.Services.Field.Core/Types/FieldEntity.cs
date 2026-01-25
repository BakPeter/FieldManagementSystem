namespace FieldManagementSystem.Services.Field.Core.Types;

public class FieldEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> AuthorizedUsers { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}