using System.Collections.Concurrent;
using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Repository;
using Microsoft.Extensions.Logging;
using Moq;

// For ConcurrentDictionary directly

namespace FieldManagementSystem.Services.Field.Tests;

[TestFixture]
public class FieldCachedRepositoryAdapterTests
{
    private Mock<ILogger<FieldCachedRepositoryAdapter>> _mockLogger;
    private FieldCachedRepositoryAdapter _adapter;
    private ConcurrentDictionary<string, FieldEntity> _privateDataField; // To access the private dictionary for setup/assertion

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<FieldCachedRepositoryAdapter>>();
        _adapter = new FieldCachedRepositoryAdapter(_mockLogger.Object);

        // Use reflection to access the private _data field
        _privateDataField = (ConcurrentDictionary<string, FieldEntity>)typeof(FieldCachedRepositoryAdapter)
            .GetField("_data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(_adapter)!;
    }

    [TearDown]
    public void Teardown()
    {
        _privateDataField.Clear(); // Clear data for each test
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _adapter = null;
        _privateDataField = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    // Constructor Test
    [Test]
    public void Constructor_InitializesWithPredefinedFields()
    {
        // Assert
        Assert.That(_privateDataField.Count, Is.EqualTo(2));
        Assert.That(_privateDataField.Values.Any(f => f.Name == "Green Field"), Is.True);
        Assert.That(_privateDataField.Values.Any(f => f.Name == "Orange Field"), Is.True);
    }

    // GetAllFieldsAsync Tests
    [Test]
    public async Task GetAllFieldsAsync_ReturnsAllFields_WhenDataExists()
    {
        // Arrange
        // Initialized in Setup with 2 fields

        // Act
        var result = await _adapter.GetAllFieldsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(f => f.Name == "Green Field"), Is.True);
        Assert.That(result.Any(f => f.Name == "Orange Field"), Is.True);
    }

    [Test]
    public async Task GetAllFieldsAsync_ReturnsEmptyCollection_WhenNoDataExists()
    {
        // Arrange
        _privateDataField.Clear();

        // Act
        var result = await _adapter.GetAllFieldsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    // GetFieldAsync Tests
    [Test]
    public async Task GetFieldAsync_ReturnsField_WhenIdExists()
    {
        // Arrange
        var existingField = _privateDataField.Values.First(); // Get one of the initial fields
        
        // Act
        var result = await _adapter.GetFieldAsync(existingField.Id.ToString());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingField.Id));
        Assert.That(result.Name, Is.EqualTo(existingField.Name));
    }

    [Test]
    public async Task GetFieldAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _adapter.GetFieldAsync(nonExistentId);

        // Assert
        Assert.That(result, Is.Null);
    }

    // GetFieldByNameAsync Tests
    [Test]
    public async Task GetFieldByNameAsync_ReturnsField_WhenNameExists()
    {
        // Arrange
        var existingField = _privateDataField.Values.First(); // Get one of the initial fields
        
        // Act
        var result = await _adapter.GetFieldByNameAsync(existingField.Name);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingField.Id));
        Assert.That(result.Name, Is.EqualTo(existingField.Name));
    }

    [Test]
    public async Task GetFieldByNameAsync_ReturnsNull_WhenNameDoesNotExist()
    {
        // Arrange
        var nonExistentName = "NonExistentField";

        // Act
        var result = await _adapter.GetFieldByNameAsync(nonExistentName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetFieldByNameAsync_ReturnsField_WhenNameExistsCaseInsensitive()
    {
        // Arrange
        var existingField = _privateDataField.Values.First();
        var caseInsensitiveName = existingField.Name.ToUpper(); // Test with upper case

        // Act
        var result = await _adapter.GetFieldByNameAsync(caseInsensitiveName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingField.Id));
        Assert.That(result.Name, Is.EqualTo(existingField.Name));
    }

    // CreateFieldAsync Tests
    [Test]
    public async Task CreateFieldAsync_SuccessfullyAddsNewField()
    {
        // Arrange
        var newField = new FieldEntity { Id = Guid.NewGuid(), Name = "NewTestField", Description = "New Desc", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };
        var initialCount = _privateDataField.Count;

        // Act
        var result = await _adapter.CreateFieldAsync(newField);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_privateDataField.Count, Is.EqualTo(initialCount + 1));
        Assert.That(_privateDataField.ContainsKey(newField.Id.ToString()), Is.True);
        Assert.That(_privateDataField[newField.Id.ToString()], Is.EqualTo(newField));
    }

    [Test]
    public async Task CreateFieldAsync_FailsToAddField_WhenIdAlreadyExists()
    {
        // Arrange
        var existingField = _privateDataField.Values.First();
        var duplicateField = new FieldEntity { Id = existingField.Id, Name = "DuplicateName", Description = "Duplicate Desc", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };
        var initialCount = _privateDataField.Count;

        // Act
        var result = await _adapter.CreateFieldAsync(duplicateField);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_privateDataField.Count, Is.EqualTo(initialCount)); // Count should not change
        // Ensure the original field remains unchanged
        Assert.That(_privateDataField[existingField.Id.ToString()].Name, Is.EqualTo(existingField.Name));
    }

    // UpdateField Tests
    [Test]
    public async Task UpdateField_SuccessfullyUpdatesExistingField()
    {
        // Arrange
        var fieldToUpdate = _privateDataField.Values.First();
        var originalName = fieldToUpdate.Name;
        fieldToUpdate.Name = "Updated Name";
        fieldToUpdate.Description = "Updated Description";
        var initialCount = _privateDataField.Count;

        // Act
        var result = await _adapter.UpdateField(fieldToUpdate);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_privateDataField.Count, Is.EqualTo(initialCount)); // Count should not change
        Assert.That(_privateDataField[fieldToUpdate.Id.ToString()].Name, Is.EqualTo("Updated Name"));
        Assert.That(_privateDataField[fieldToUpdate.Id.ToString()].Description, Is.EqualTo("Updated Description"));
    }

    [Test]
    public async Task UpdateField_AddsField_WhenIdDoesNotExist()
    {
        // Arrange
        var newField = new FieldEntity { Id = Guid.NewGuid(), Name = "NonExistentUpdatedField", Description = "Desc", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };
        var initialCount = _privateDataField.Count;

        // Act
        var result = await _adapter.UpdateField(newField);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_privateDataField.Count, Is.EqualTo(initialCount + 1));
        Assert.That(_privateDataField.ContainsKey(newField.Id.ToString()), Is.True);
        Assert.That(_privateDataField[newField.Id.ToString()], Is.EqualTo(newField));
    }

    // DeleteField Tests
    [Test]
    public async Task DeleteField_SuccessfullyDeletesExistingField()
    {
        // Arrange
        var fieldToDelete = _privateDataField.Values.First();
        var initialCount = _privateDataField.Count;

        // Act
        var result = await _adapter.DeleteField(fieldToDelete.Id.ToString());

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_privateDataField.Count, Is.EqualTo(initialCount - 1));
        Assert.That(_privateDataField.ContainsKey(fieldToDelete.Id.ToString()), Is.False);
    }

    [Test]
    public async Task DeleteField_ReturnsFalse_WhenIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();
        var initialCount = _privateDataField.Count;

        // Act
        var result = await _adapter.DeleteField(nonExistentId);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_privateDataField.Count, Is.EqualTo(initialCount)); // Count should not change
    }
}
