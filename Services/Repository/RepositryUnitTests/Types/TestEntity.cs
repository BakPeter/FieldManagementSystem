namespace RepositoryUnitTests.Types;

public class TestEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public List<int> Values { get; set; } = [];
    public StatusType Status { get; set; }
    public SubEntity? Nested { get; set; }
    public List<SubEntity> NestedList { get; set; } = [];
}

public class SubEntity
{
    public string Key { get; set; } = string.Empty;
    public int Value { get; set; }
}

public enum StatusType
{
    Active,
    Inactive,
    Pending
}