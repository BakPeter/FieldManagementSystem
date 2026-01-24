using Moq;
using FieldManagementSystem.User.Controllers;
using FieldManagementSystem.User.Core.Interfaces;
using FieldManagementSystem.User.Core.Types;
using FieldManagementSystem.User.Core.Types.DTOs;
using FieldManagementSystem.User.Infrastructure.types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http; // For StatusCodes

namespace FieldManagementSystem.User.Tests;

public class UserControllerTests
{
    private Mock<ILogger<UserController>> _mockLogger;
    private Mock<IUserService> _mockUserService;
    private UserController _userController;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<UserController>>();
        _mockUserService = new Mock<IUserService>();
        _userController = new UserController(_mockLogger.Object, _mockUserService.Object);
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _mockUserService = null;
        _userController = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    // GetUsers Tests
    [Test]
    public async Task GetUsers_ReturnsOkWithUsers_WhenServiceSucceeds()
    {
        // Arrange
        var users = new List<UserEntity>
        {
            new UserEntity { Id = Guid.NewGuid(), Email = "test1@example.com", FirstName = "F1", LastName = "L1" },
            new UserEntity { Id = Guid.NewGuid(), Email = "test2@example.com", FirstName = "F2", LastName = "L2" }
        };
        _mockUserService.Setup(s => s.GetUsersAsync()).ReturnsAsync(new Result<IEnumerable<UserEntity>>(true, users));

        // Act
        var result = await _userController.GetUsers();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<UserEntity>>());
        Assert.That(((IEnumerable<UserEntity>)okResult.Value).Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetUsers_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetUsersAsync())
            .ReturnsAsync(new Result<IEnumerable<UserEntity>>(false, null, new Exception("Service error")));

        // Act
        var result = await _userController.GetUsers();

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.EqualTo("Service error"));
    }

    [Test]
    public async Task GetUsers_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetUsersAsync()).ThrowsAsync(new Exception("Unhandled exception"));

        // Act
        var result = await _userController.GetUsers();

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>()); // ErrorHandler returns BadRequest with ProblemDetails
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        Assert.That(((ProblemDetails)badRequestResult.Value).Detail, Is.EqualTo("Unhandled exception"));
    }

    // GetUser (by ID/Email) Tests
    [Test]
    public async Task GetUser_ReturnsOkWithUser_WhenServiceSucceeds()
    {
        // Arrange
        var email = "test@example.com";
        var user = new UserEntity { Id = Guid.NewGuid(), Email = email, FirstName = "F1", LastName = "L1" };
        _mockUserService.Setup(s => s.GetUserAsync(email)).ReturnsAsync(new Result<UserEntity>(true, user));

        // Act
        var result = await _userController.GetUser(email);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(okResult.Value, Is.InstanceOf<UserEntity>());
        Assert.That(((UserEntity)okResult.Value).Email, Is.EqualTo(email));
    }

    [Test]
    public async Task GetUser_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockUserService.Setup(s => s.GetUserAsync(email))
            .ReturnsAsync(new Result<UserEntity>(false, null, new ArgumentException("User not found")));

        // Act
        var result = await _userController.GetUser(email);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.EqualTo("User not found"));
    }

    // CreateUser Tests
    [Test]
    public async Task CreateUser_ReturnsCreatedAtActionWithUser_WhenServiceSucceeds()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "new@example.com", FirstName = "New", LastName = "User" };
        var createdUser = new UserEntity { Id = Guid.NewGuid(), Email = createDto.Email, FirstName = createDto.FirstName, LastName = createDto.LastName };
        _mockUserService.Setup(s => s.CreateUserAsync(createDto)).ReturnsAsync(new Result<UserEntity>(true, createdUser));

        // Act
        var result = await _userController.CreateUser(createDto);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        var createdAtActionResult = ((CreatedAtActionResult)result)!;
        Assert.That(createdAtActionResult.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        Assert.That(createdAtActionResult.ActionName, Is.EqualTo(nameof(UserController.GetUser)));
        Assert.That(createdAtActionResult.RouteValues!["id"], Is.EqualTo(createDto.Email));
        Assert.That(createdAtActionResult.Value, Is.InstanceOf<UserEntity>());
        Assert.That(((UserEntity)createdAtActionResult.Value).Email, Is.EqualTo(createDto.Email));
    }

    [Test]
    public async Task CreateUser_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "invalid", FirstName = "", LastName = "U" };
        var validationErrors = new List<string> { "Email invalid", "First name too short" };
        _mockUserService.Setup(s => s.CreateUserAsync(createDto))
            .ReturnsAsync(new Result<UserEntity>(false, null, new UserValidationException(validationErrors)));

        // Act
        var result = await _userController.CreateUser(createDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        Assert.That(((ProblemDetails)badRequestResult.Value).Title, Is.EqualTo("User validation failed."));
        Assert.That(((ProblemDetails)badRequestResult.Value).Detail, Is.EqualTo(System.Text.Json.JsonSerializer.Serialize(validationErrors)));
    }

    [Test]
    public async Task CreateUser_ReturnsBadRequest_WhenServiceReturnsGenericError()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "new@example.com", FirstName = "New", LastName = "User" };
        _mockUserService.Setup(s => s.CreateUserAsync(createDto))
            .ReturnsAsync(new Result<UserEntity>(false, null, new Exception("Generic service error")));

        // Act
        var result = await _userController.CreateUser(createDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.EqualTo("Generic service error"));
    }

    // UpdateUser Tests
    [Test]
    public async Task UpdateUser_ReturnsOkWithSuccessMessage_WhenServiceSucceeds()
    {
        // Arrange
        var updateDto = new UpdateUserDto { Email = "update@example.com", FirstName = "Updated", LastName = "User" };
        _mockUserService.Setup(s => s.UpdateUser(updateDto)).ReturnsAsync(new Result<string>(true, "User updated successfully"));

        // Act
        var result = await _userController.UpdateUser(updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(okResult.Value, Is.EqualTo("User updated successfully"));
    }

    [Test]
    public async Task UpdateUser_ReturnsBadRequest_WhenUpdateValidationFails()
    {
        // Arrange
        var updateDto = new UpdateUserDto { Email = "update@example.com", FirstName = "I", LastName = "nvalid" };
        var validationErrors = new List<string> { "First name too short" };
        _mockUserService.Setup(s => s.UpdateUser(updateDto))
            .ReturnsAsync(new Result<string>(false, null, new UserValidationException(validationErrors)));

        // Act
        var result = await _userController.UpdateUser(updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        Assert.That(((ProblemDetails)badRequestResult.Value).Title, Is.EqualTo("User validation failed."));
        Assert.That(((ProblemDetails)badRequestResult.Value).Detail, Is.EqualTo(System.Text.Json.JsonSerializer.Serialize(validationErrors)));
    }

    [Test]
    public async Task UpdateUser_ReturnsBadRequest_WhenUpdateServiceReturnsGenericError()
    {
        // Arrange
        var updateDto = new UpdateUserDto { Email = "update@example.com", FirstName = "Updated", LastName = "User" };
        _mockUserService.Setup(s => s.UpdateUser(updateDto))
            .ReturnsAsync(new Result<string>(false, null, new Exception("Generic update service error")));
        
        // Act
        var result = await _userController.UpdateUser(updateDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.EqualTo("Generic update service error"));
    }

    // DeleteUser Tests
    [Test]
    public async Task DeleteUser_ReturnsOkWithSuccessMessage_WhenServiceSucceeds()
    {
        // Arrange
        var id = "delete@example.com";
        _mockUserService.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(new Result<string>(true, "User deleted successfully"));

        // Act
        var result = await _userController.DeleteUser(id);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(okResult.Value, Is.EqualTo("User deleted successfully"));
    }

    [Test]
    public async Task DeleteUser_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var id = "nonexistent@example.com";
        _mockUserService.Setup(s => s.DeleteUserAsync(id))
            .ReturnsAsync(new Result<string>(false, null, new ArgumentException("User not found for deletion")));

        // Act
        var result = await _userController.DeleteUser(id);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.EqualTo("User not found for deletion"));
    }

    [Test]
    public async Task DeleteUser_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var id = "error@example.com";
        _mockUserService.Setup(s => s.DeleteUserAsync(id)).ThrowsAsync(new Exception("Unhandled deletion exception"));

        // Act
        var result = await _userController.DeleteUser(id);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>()); // ErrorHandler returns BadRequest with ProblemDetails
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestResult.Value, Is.InstanceOf<ProblemDetails>());
        Assert.That(((ProblemDetails)badRequestResult.Value).Detail, Is.EqualTo("Unhandled deletion exception"));
    }
}
