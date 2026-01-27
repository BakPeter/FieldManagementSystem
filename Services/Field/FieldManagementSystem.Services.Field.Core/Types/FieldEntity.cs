using System.Text.Json.Serialization;

namespace FieldManagementSystem.Services.Field.Core.Types;

public class FieldEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonIgnore] public ICollection<FieldAuthorizedUserRow> AuthorizedUserLinks { get; set; } = new List<FieldAuthorizedUserRow>();
    public IEnumerable<string> AuthorizedUsers
    {
        get => AuthorizedUserLinks.Select(x => x.UserId.ToString());
        set
        {
            AuthorizedUserLinks = (value ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => new FieldAuthorizedUserRow { UserId = Guid.Parse(s) })
                .ToList();
        }
    }

    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public sealed class FieldAuthorizedUserRow
{
    public Guid FieldId { get; set; }
    public Guid UserId { get; set; }
}