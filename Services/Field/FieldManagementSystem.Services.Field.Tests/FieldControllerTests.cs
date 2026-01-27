using FieldManagementSystem.Services.Field.Controllers;
using FieldManagementSystem.Services.Field.Core.Interfaces;
using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Core.Types.DTOs;
using FieldManagementSystem.Services.Field.Infrastructure.types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FieldManagementSystem.Services.Field.Tests;

[TestFixture]
public class FieldControllerTests
{
    private Mock<ILogger<FieldController>> _mockLogger;
    private Mock<IFieldService> _mockFieldService;
    private FieldController _fieldController;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<FieldController>>();
        _mockFieldService = new Mock<IFieldService>();
        _fieldController = new FieldController(_mockLogger.Object, _mockFieldService.Object);
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _mockFieldService = null;
        _fieldController = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    // GetFields Tests
    [Test]
    public async Task GetFields_ReturnsOkWithData_WhenServiceSucceeds()
    {
        // Arrange
        var fields = new List<FieldEntity>
        {
            new() { Id = Guid.NewGuid(), Name = "Field1" },
            new() { Id = Guid.NewGuid(), Name = "Field2" }
        };
        _mockFieldService.Setup(s => s.GetFieldsAsync(default)).ReturnsAsync(new Result<IEnumerable<FieldEntity>>(true, fields));

        // Act
        var result = await _fieldController.GetFields();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<FieldEntity>>());
        Assert.That((okResult.Value as IEnumerable<FieldEntity>)?.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetFields_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var errorMessage = "Service failed to get fields";
        _mockFieldService.Setup(s => s.GetFieldsAsync(default)).ReturnsAsync(new Result<IEnumerable<FieldEntity>>(false, null, new Exception(errorMessage)));

        // Act
        var result = await _fieldController.GetFields();

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task GetFields_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var exceptionMessage = "An unexpected error occurred";
        _mockFieldService.Setup(s => s.GetFieldsAsync(default)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fieldController.GetFields();

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        var problemDetails = badRequestResult.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(problemDetails.Detail, Is.EqualTo(exceptionMessage));
    }

    // GetField Tests
    [Test]
    public async Task GetField_ReturnsOkWithData_WhenServiceSucceeds()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var field = new FieldEntity { Id = fieldId, Name = "TestField" };
        _mockFieldService.Setup(s => s.GetFieldAsync(fieldId.ToString(), CancellationToken.None)).ReturnsAsync(new Result<FieldEntity>(true, field));

        // Act
        var result = await _fieldController.GetField(fieldId.ToString());

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<FieldEntity>());
        Assert.That((okResult.Value as FieldEntity)?.Id, Is.EqualTo(fieldId));
    }

    [Test]
    public async Task GetField_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var errorMessage = $"Field with id {fieldId} not found";
        _mockFieldService.Setup(s => s.GetFieldAsync(fieldId.ToString(), CancellationToken.None)).ReturnsAsync(new Result<FieldEntity>(false, null, new ArgumentException(errorMessage)));

        // Act
        var result = await _fieldController.GetField(fieldId.ToString());

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task GetField_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var exceptionMessage = "An unexpected error occurred during GetField";
        _mockFieldService.Setup(s => s.GetFieldAsync(fieldId.ToString(), CancellationToken.None)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fieldController.GetField(fieldId.ToString());

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        var problemDetails = badRequestResult.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(problemDetails.Detail, Is.EqualTo(exceptionMessage));
    }

    // CreateField Tests
    [Test]
    public async Task CreateField_ReturnsCreatedAtAction_WhenServiceSucceeds()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "NewField", Description = "Desc" };
        var createdField = new FieldEntity { Id = Guid.NewGuid(), Name = createDto.Name, Description = createDto.Description };
        _mockFieldService.Setup(s => s.CreateFieldAsync(createDto, CancellationToken.None)).ReturnsAsync(new Result<FieldEntity>(true, createdField));

        // Act
        var result = await _fieldController.CreateField(createDto);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        var createdAtActionResult = result as CreatedAtActionResult;
        Assert.That(createdAtActionResult, Is.Not.Null);
        Assert.That(createdAtActionResult.ActionName, Is.EqualTo(nameof(FieldController.GetField)));
        Assert.That(createdAtActionResult.RouteValues["id"], Is.EqualTo(createdField.Id));
        Assert.That(createdAtActionResult.Value, Is.InstanceOf<FieldEntity>());
        Assert.That((createdAtActionResult.Value as FieldEntity)?.Name, Is.EqualTo(createDto.Name));
    }

    [Test]
    public async Task CreateField_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "NF" };
        var validationErrors = new List<string> { "Name too short" };
        var validationException = new FieldValidationException(validationErrors);
        _mockFieldService.Setup(s => s.CreateFieldAsync(createDto, CancellationToken.None)).ReturnsAsync(new Result<FieldEntity>(false, null, validationException));

        // Act
        var result = await _fieldController.CreateField(createDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        var problemDetails = badRequestResult.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(problemDetails.Title, Is.EqualTo(validationException.Message));
        Assert.That(problemDetails.Detail, Is.EqualTo(System.Text.Json.JsonSerializer.Serialize(validationErrors)));
    }

    [Test]
    public async Task CreateField_ReturnsBadRequest_WhenServiceFailsWithGenericError()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "NewField" };
        var errorMessage = "Failed to create field";
        _mockFieldService.Setup(s => s.CreateFieldAsync(createDto, CancellationToken.None)).ReturnsAsync(new Result<FieldEntity>(false, null, new Exception(errorMessage)));

        // Act
        var result = await _fieldController.CreateField(createDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task CreateField_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var createDto = new CreateFieldDto { Name = "NewField" };
        var exceptionMessage = "An unexpected error occurred during CreateField";
        _mockFieldService.Setup(s => s.CreateFieldAsync(createDto, CancellationToken.None)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fieldController.CreateField(createDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        var problemDetails = badRequestResult.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(problemDetails.Detail, Is.EqualTo(exceptionMessage));
    }

    // UpdateField Tests
    [Test]
    public async Task UpdateField_ReturnsOkWithData_WhenServiceSucceeds()
    {
        // Arrange
        var updateDto = new UpdateFieldDto { Id = Guid.NewGuid().ToString(), Description = "UpdatedField" };
        var successMessage = $"Field with id {updateDto.Id} updated";
        _mockFieldService.Setup(s => s.UpdateField(updateDto, CancellationToken.None)).ReturnsAsync(new Result<string>(true, successMessage));

        // Act
        var result = await _fieldController.UpdateField(updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(successMessage));
    }

    [Test]
    public async Task UpdateField_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var updateDto = new UpdateFieldDto { Id = Guid.NewGuid().ToString(), Description = "U" };
        var validationErrors = new List<string> { "Name too short" };
        var validationException = new FieldValidationException(validationErrors);
        _mockFieldService.Setup(s => s.UpdateField(updateDto, CancellationToken.None)).ReturnsAsync(new Result<string>(false, null, validationException));

        // Act
        var result = await _fieldController.UpdateField(updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        var problemDetails = badRequestResult.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(problemDetails.Title, Is.EqualTo(validationException.Message));
        Assert.That(problemDetails.Detail, Is.EqualTo(System.Text.Json.JsonSerializer.Serialize(validationErrors)));
    }

    [Test]
    public async Task UpdateField_ReturnsBadRequest_WhenServiceFailsWithGenericError()
    {
        // Arrange
        var updateDto = new UpdateFieldDto { Id = Guid.NewGuid().ToString(), Description = "UpdatedField" };
        var errorMessage = "Failed to update field";
        _mockFieldService.Setup(s => s.UpdateField(updateDto, CancellationToken.None)).ReturnsAsync(new Result<string>(false, null, new Exception(errorMessage)));

        // Act
        var result = await _fieldController.UpdateField(updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task UpdateField_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var updateDto = new UpdateFieldDto { Id = Guid.NewGuid().ToString(), Description = "UpdatedField" };
        var exceptionMessage = "An unexpected error occurred during UpdateField";
        _mockFieldService.Setup(s => s.UpdateField(updateDto, CancellationToken.None)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fieldController.UpdateField(updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        var problemDetails = badRequestResult.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(problemDetails.Detail, Is.EqualTo(exceptionMessage));
    }

    // DeleteField Tests
    [Test]
    public async Task DeleteField_ReturnsOkWithData_WhenServiceSucceeds()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        var successMessage = $"Field {fieldId} deleted";
        _mockFieldService.Setup(s => s.DeleteFieldAsync(fieldId, CancellationToken.None)).ReturnsAsync(new Result<string>(true, successMessage));

        // Act
        var result = await _fieldController.DeleteField(fieldId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(successMessage));
    }

    [Test]
    public async Task DeleteField_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        var errorMessage = $"Field with id {fieldId} not found";
        _mockFieldService.Setup(s => s.DeleteFieldAsync(fieldId, CancellationToken.None)).ReturnsAsync(new Result<string>(false, null, new ArgumentException(errorMessage)));

        // Act
        var result = await _fieldController.DeleteField(fieldId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task DeleteField_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var fieldId = Guid.NewGuid().ToString();
        var exceptionMessage = "An unexpected error occurred during DeleteField";
        _mockFieldService.Setup(s => s.DeleteFieldAsync(fieldId, CancellationToken.None)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fieldController.DeleteField(fieldId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        var problemDetails = badRequestResult.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(problemDetails.Detail, Is.EqualTo(exceptionMessage));
    }
}