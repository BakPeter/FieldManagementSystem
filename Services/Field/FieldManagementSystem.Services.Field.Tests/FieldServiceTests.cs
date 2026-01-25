using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Core.Types.DTOs;
using FieldManagementSystem.Services.Field.Infrastructure.Services;
using FieldManagementSystem.Services.Field.Infrastructure.types;
using Microsoft.Extensions.Logging;
using Moq;

namespace FieldManagementSystem.Services.Field.Tests;

[TestFixture]
public class FieldServiceTests
{
    private Mock<ILogger<FieldService>> _mockLogger;
    private Mock<IFieldRepository> _mockRepository;
    private Mock<IFieldValidation> _mockFieldValidation;
    private FieldService _fieldService;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<FieldService>>();
        _mockRepository = new Mock<IFieldRepository>();
        _mockFieldValidation = new Mock<IFieldValidation>();
        _fieldService = new FieldService(_mockLogger.Object, _mockRepository.Object, _mockFieldValidation.Object);
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _mockRepository = null;
        _mockFieldValidation = null;
        _fieldService = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Test]
    public async Task GetFieldsAsync_ReturnsListOfFields_WhenFieldsExist()
    {
        // Arrange
        var fields = new List<FieldEntity>
        {
            new() { Id = Guid.NewGuid(), Name = "Field1", Description = "Desc1", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Field2", Description = "Desc2", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
        };
        _mockRepository.Setup(r => r.GetAllFieldsAsync()).ReturnsAsync(fields);

        // Act
        var result = await _fieldService.GetFieldsAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data, Is.EqualTo(fields));
        Assert.That(result.Data.Count(), Is.EqualTo(2));
        // _mockLogger.Verify(
        //     x => x.Log(
        //         LogLevel.Information,
        //         It.IsAny<EventId>(),
        //         It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Get Fields- Success: True")),
        //         It.IsAny<Exception>(),
        //         (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        //     Times.Once);
    }

    [Test]
    public async Task GetFieldsAsync_ReturnsEmptyList_WhenNoFieldsExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllFieldsAsync()).ReturnsAsync(new List<FieldEntity>());

        // Act
        var result = await _fieldService.GetFieldsAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data, Is.Empty);
        // _mockLogger.Verify(
        //     x => x.Log(
        //         LogLevel.Information,
        //         It.IsAny<EventId>(),
        //         It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Get Fields- Success: True")),
        //         It.IsAny<Exception>(),
        //         (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        //     Times.Once);
    }

    [Test]
    public async Task GetFieldAsync_ReturnsField_WhenFieldExists()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        var field = new FieldEntity { Id = Guid.Parse(fieldId), Name = "TestField", Description = "Desc", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetFieldAsync(fieldId)).ReturnsAsync(field);

        // Act
        var result = await _fieldService.GetFieldAsync(fieldId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Id.ToString(), Is.EqualTo(fieldId));
        // _mockLogger.Verify(
        //     x => x.Log(
        //         LogLevel.Information,
        //         It.IsAny<EventId>(),
        //         It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Get Field - id: {fieldId} Success: True")),
        //         It.IsAny<Exception>(),
        //         (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        //     Times.Once);
    }

    [Test]
    public async Task GetFieldAsync_ReturnsError_WhenFieldDoesNotExist()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        _mockRepository.Setup(r => r.GetFieldAsync(fieldId)).ReturnsAsync(null as FieldEntity);

        // Act
        var result = await _fieldService.GetFieldAsync(fieldId);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"Field with id {fieldId} not found (Parameter 'id')"));
        // _mockLogger.Verify(
        //     x => x.Log(
        //         LogLevel.Information,
        //         It.IsAny<EventId>(),
        //         It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Get Field - id: {fieldId} Success: False")),
        //         It.IsAny<Exception>(),
        //         (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        //     Times.Once);
    }

    // CreateFieldAsync Tests
    [Test]
    public async Task CreateFieldAsync_ReturnsFieldEntity_WhenValidationPassesAndFieldAdded()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "NewField", Description = "NewDesc", AuthorizedUsers = new List<string> { "user1" } };
        _mockFieldValidation.Setup(v => v.Validate(It.IsAny<FieldEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.GetFieldByNameAsync(createDto.Name)).ReturnsAsync(null as FieldEntity);
        _mockRepository.Setup(r => r.CreateFieldAsync(It.IsAny<FieldEntity>())).ReturnsAsync(true);

        // Act
        var result = await _fieldService.CreateFieldAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Name, Is.EqualTo(createDto.Name));
        _mockRepository.Verify(r => r.CreateFieldAsync(It.IsAny<FieldEntity>()), Times.Once);
    }

    [Test]
    public async Task CreateFieldAsync_ReturnsError_WhenValidationFails()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "NF", Description = "", AuthorizedUsers = new List<string>() };
        var validationErrors = new List<string> { "Name too short", "Description empty" };
        _mockFieldValidation.Setup(v => v.Validate(It.IsAny<FieldEntity>(), out It.Ref<IEnumerable<string>>.IsAny))
            .Callback((FieldEntity _, out IEnumerable<string> errors) => { errors = validationErrors; })
            .Returns(false);

        // Act
        var result = await _fieldService.CreateFieldAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<FieldValidationException>());
        Assert.That(((FieldValidationException)result.Error).ValidationErrors, Is.EqualTo(validationErrors));
        _mockRepository.Verify(r => r.CreateFieldAsync(It.IsAny<FieldEntity>()), Times.Never);
    }

    [Test]
    public async Task CreateFieldAsync_ReturnsError_WhenFieldAlreadyExists()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "ExistingField", Description = "Desc", AuthorizedUsers = new List<string>() };
        var existingField = new FieldEntity { Id = Guid.NewGuid(), Name = createDto.Name };
        _mockFieldValidation.Setup(v => v.Validate(It.IsAny<FieldEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.GetFieldByNameAsync(createDto.Name)).ReturnsAsync(existingField);

        // Act
        var result = await _fieldService.CreateFieldAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"Field with the name {createDto.Name} exists (Parameter 'createFieldDto')"));
        _mockRepository.Verify(r => r.CreateFieldAsync(It.IsAny<FieldEntity>()), Times.Never);
    }

    [Test]
    public async Task CreateFieldAsync_ReturnsError_WhenRepositoryFailsToAddField()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "NewField", Description = "NewDesc", AuthorizedUsers = new List<string>() };
        _mockFieldValidation.Setup(v => v.Validate(It.IsAny<FieldEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.GetFieldByNameAsync(createDto.Name)).ReturnsAsync(null as FieldEntity);
        _mockRepository.Setup(r => r.CreateFieldAsync(It.IsAny<FieldEntity>())).ReturnsAsync(false);

        // Act
        var result = await _fieldService.CreateFieldAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<Exception>());
        Assert.That(result.Error.Message, Is.EqualTo("Failed to add Field to repository."));
        _mockRepository.Verify(r => r.CreateFieldAsync(It.IsAny<FieldEntity>()), Times.Once);
    }

    // UpdateField Tests
    [Test]
    public async Task UpdateField_ReturnsSuccessMessage_WhenFieldExistsAndValidationPassesAndFieldUpdated()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var updateDto = new UpdateFieldDto { Id = fieldId.ToString(), Description = "UpdatedDesc", AuthorizedUsers = new List<string> { "user2" } };
        var existingField = new FieldEntity { Id = fieldId, Name = "ExistingField", Description = "OldDesc", AuthorizedUsers = new List<string> { "user1" }, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetFieldAsync(fieldId.ToString())).ReturnsAsync(existingField);
        _mockFieldValidation.Setup(v => v.Validate(It.IsAny<FieldEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.UpdateField(It.IsAny<FieldEntity>())).ReturnsAsync(true);

        // Act
        var result = await _fieldService.UpdateField(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.EqualTo($"Field with id {fieldId} updated"));
        _mockRepository.Verify(r => r.UpdateField(It.Is<FieldEntity>(f => f.Id == fieldId && f.Description == updateDto.Description && f.AuthorizedUsers.Contains("user2"))), Times.Once);
    }

    [Test]
    public async Task UpdateField_ReturnsError_WhenFieldDoesNotExist()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var updateDto = new UpdateFieldDto { Id = fieldId.ToString(), Description = "UpdatedDesc", AuthorizedUsers = new List<string>() };
        _mockRepository.Setup(r => r.GetFieldAsync(fieldId.ToString())).ReturnsAsync(null as FieldEntity);

        // Act
        var result = await _fieldService.UpdateField(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"Field with id {fieldId} not found (Parameter 'updateFieldDto')"));
        _mockRepository.Verify(r => r.UpdateField(It.IsAny<FieldEntity>()), Times.Never);
    }

    [Test]
    public async Task UpdateField_ReturnsError_WhenValidationFails()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var updateDto = new UpdateFieldDto { Id = fieldId.ToString(), Description = "S", AuthorizedUsers = new List<string>() };
        var existingField = new FieldEntity { Id = fieldId, Name = "ExistingField", Description = "OldDesc", AuthorizedUsers = new List<string>(), CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };
        var validationErrors = new List<string> { "Description too short" };

        _mockRepository.Setup(r => r.GetFieldAsync(fieldId.ToString())).ReturnsAsync(existingField);
        _mockFieldValidation.Setup(v => v.Validate(It.IsAny<FieldEntity>(), out It.Ref<IEnumerable<string>>.IsAny))
            .Callback((FieldEntity _, out IEnumerable<string> errors) => { errors = validationErrors; })
            .Returns(false);

        // Act
        var result = await _fieldService.UpdateField(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<FieldValidationException>());
        Assert.That(((FieldValidationException)result.Error).ValidationErrors, Is.EqualTo(validationErrors));
        _mockRepository.Verify(r => r.UpdateField(It.IsAny<FieldEntity>()), Times.Never);
    }

    [Test]
    public async Task UpdateField_ReturnsError_WhenRepositoryFailsToUpdate()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var updateDto = new UpdateFieldDto { Id = fieldId.ToString(), Description = "UpdatedDesc", AuthorizedUsers = new List<string>() };
        var existingField = new FieldEntity { Id = fieldId, Name = "ExistingField", Description = "OldDesc", AuthorizedUsers = new List<string>(), CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetFieldAsync(fieldId.ToString())).ReturnsAsync(existingField);
        _mockFieldValidation.Setup(v => v.Validate(It.IsAny<FieldEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.UpdateField(It.IsAny<FieldEntity>())).ReturnsAsync(false);

        // Act
        var result = await _fieldService.UpdateField(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<Exception>());
        Assert.That(result.Error.Message, Is.EqualTo($"Field {fieldId} update failed"));
        _mockRepository.Verify(r => r.UpdateField(It.IsAny<FieldEntity>()), Times.Once);
    }

    // DeleteFieldAsync Tests
    [Test]
    public async Task DeleteFieldAsync_ReturnsSuccessMessage_WhenFieldExistsAndDeleted()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var existingField = new FieldEntity { Id = fieldId, Name = "FieldToDelete", Description = "Desc", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetFieldAsync(fieldId.ToString())).ReturnsAsync(existingField);
        _mockRepository.Setup(r => r.DeleteField(fieldId.ToString())).ReturnsAsync(true);

        // Act
        var result = await _fieldService.DeleteFieldAsync(fieldId.ToString());

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.EqualTo($"Field {fieldId} deleted"));
        _mockRepository.Verify(r => r.DeleteField(fieldId.ToString()), Times.Once);
    }

    [Test]
    public async Task DeleteFieldAsync_ReturnsError_WhenFieldDoesNotExist()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetFieldAsync(fieldId.ToString())).ReturnsAsync(null as FieldEntity);

        // Act
        var result = await _fieldService.DeleteFieldAsync(fieldId.ToString());

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"Field with id {fieldId} not found (Parameter 'id')"));
        _mockRepository.Verify(r => r.DeleteField(fieldId.ToString()), Times.Never);
    }

    [Test]
    public async Task DeleteFieldAsync_ReturnsError_WhenRepositoryFailsToDelete()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var existingField = new FieldEntity { Id = fieldId, Name = "FieldToDelete", Description = "Desc", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetFieldAsync(fieldId.ToString())).ReturnsAsync(existingField);
        _mockRepository.Setup(r => r.DeleteField(fieldId.ToString())).ReturnsAsync(false);

        // Act
        var result = await _fieldService.DeleteFieldAsync(fieldId.ToString());

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<Exception>());
        Assert.That(result.Error.Message, Is.EqualTo($"Field with id {fieldId} delete failed"));
        _mockRepository.Verify(r => r.DeleteField(fieldId.ToString()), Times.Once);
    }
}
