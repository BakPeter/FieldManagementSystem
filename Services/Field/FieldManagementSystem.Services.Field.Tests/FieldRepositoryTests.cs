using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Repository;
using Microsoft.Extensions.Logging;
using Moq;

namespace FieldManagementSystem.Services.Field.Tests;

[TestFixture]
public class FieldRepositoryTests
{
    private Mock<ILogger<FieldRepository>> _mockLogger;
    private Mock<IFieldRepositoryAdapter> _mockAdapter;
    private FieldRepository _fieldRepository;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<FieldRepository>>();
        _mockAdapter = new Mock<IFieldRepositoryAdapter>();
        _fieldRepository = new FieldRepository(_mockLogger.Object, _mockAdapter.Object);
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _mockAdapter = null;
        _fieldRepository = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    // Constructor Test
    [Test]
    public void Constructor_InitializesDependencies()
    {
        // Assert
        Assert.That(_fieldRepository, Is.Not.Null);
    }

    // GetAllFieldsAsync Test
    [Test]
    public async Task GetAllFieldsAsync_DelegatesToAdapter_AndReturnsResult()
    {
        // Arrange
        var expectedFields = new List<FieldEntity> { new() { Id = Guid.NewGuid() } };
        _mockAdapter.Setup(a => a.GetAllFieldsAsync(CancellationToken.None)).ReturnsAsync(expectedFields);

        // Act
        var result = await _fieldRepository.GetAllFieldsAsync(CancellationToken.None);

        // Assert
        _mockAdapter.Verify(a => a.GetAllFieldsAsync(CancellationToken.None), Times.Once);
        Assert.That(result, Is.EqualTo(expectedFields));
    }

    // GetFieldByNameAsync Test
    [Test]
    public async Task GetFieldByNameAsync_DelegatesToAdapter_AndReturnsResult()
    {
        // Arrange
        var fieldName = "TestField";
        var expectedField = new FieldEntity { Id = Guid.NewGuid(), Name = fieldName };
        _mockAdapter.Setup(a => a.GetFieldByNameAsync(fieldName, CancellationToken.None)).ReturnsAsync(expectedField);

        // Act
        var result = await _fieldRepository.GetFieldByNameAsync(fieldName);

        // Assert
        _mockAdapter.Verify(a => a.GetFieldByNameAsync(fieldName, CancellationToken.None), Times.Once);
        Assert.That(result, Is.EqualTo(expectedField));
    }

    [Test]
    public async Task GetFieldByNameAsync_ReturnsNull_WhenAdapterReturnsNull()
    {
        // Arrange
        var fieldName = "NonExistent";
        _mockAdapter.Setup(a => a.GetFieldByNameAsync(fieldName, CancellationToken.None)).ReturnsAsync(null as FieldEntity);

        // Act
        var result = await _fieldRepository.GetFieldByNameAsync(fieldName);

        // Assert
        _mockAdapter.Verify(a => a.GetFieldByNameAsync(fieldName, CancellationToken.None), Times.Once);
        Assert.That(result, Is.Null);
    }

    // GetFieldAsync Test
    [Test]
    public async Task GetFieldAsync_DelegatesToAdapter_AndReturnsResult()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        var expectedField = new FieldEntity { Id = Guid.Parse(fieldId) };
        _mockAdapter.Setup(a => a.GetFieldAsync(fieldId, CancellationToken.None)).ReturnsAsync(expectedField);

        // Act
        var result = await _fieldRepository.GetFieldAsync(fieldId, CancellationToken.None);

        // Assert
        _mockAdapter.Verify(a => a.GetFieldAsync(fieldId, CancellationToken.None), Times.Once);
        Assert.That(result, Is.EqualTo(expectedField));
    }

    [Test]
    public async Task GetFieldAsync_ReturnsNull_WhenAdapterReturnsNull()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        _mockAdapter.Setup(a => a.GetFieldAsync(fieldId, CancellationToken.None)).ReturnsAsync(null as FieldEntity);

        // Act
        var result = await _fieldRepository.GetFieldAsync(fieldId, CancellationToken.None);

        // Assert
        _mockAdapter.Verify(a => a.GetFieldAsync(fieldId, CancellationToken.None), Times.Once);
        Assert.That(result, Is.Null);
    }

    // CreateFieldAsync Test
    [Test]
    public async Task CreateFieldAsync_DelegatesToAdapter_AndReturnsResult()
    {
        // Arrange
        var fieldToAdd = new FieldEntity { Id = Guid.NewGuid(), Name = "NewField" };
        _mockAdapter.Setup(a => a.CreateFieldAsync(fieldToAdd, CancellationToken.None)).ReturnsAsync(true);

        // Act
        var result = await _fieldRepository.CreateFieldAsync(fieldToAdd);

        // Assert
        _mockAdapter.Verify(a => a.CreateFieldAsync(fieldToAdd, CancellationToken.None), Times.Once);
        Assert.That(result, Is.True);
    }

    // UpdateField Test
    [Test]
    public async Task UpdateField_DelegatesToAdapter_AndReturnsResult()
    {
        // Arrange
        var fieldToUpdate = new FieldEntity { Id = Guid.NewGuid(), Name = "UpdatedField" };
        _mockAdapter.Setup(a => a.UpdateField(fieldToUpdate, CancellationToken.None)).ReturnsAsync(true);

        // Act
        var result = await _fieldRepository.UpdateField(fieldToUpdate);

        // Assert
        _mockAdapter.Verify(a => a.UpdateField(fieldToUpdate, CancellationToken.None), Times.Once);
        Assert.That(result, Is.True);
    }

    // DeleteField Test
    [Test]
    public async Task DeleteField_DelegatesToAdapter_AndReturnsResult()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        _mockAdapter.Setup(a => a.DeleteField(fieldId, CancellationToken.None)).ReturnsAsync(true);

        // Act
        var result = await _fieldRepository.DeleteField(fieldId);

        // Assert
        _mockAdapter.Verify(a => a.DeleteField(fieldId, CancellationToken.None), Times.Once);
        Assert.That(result, Is.True);
    }
}
