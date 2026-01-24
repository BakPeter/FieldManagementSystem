using FieldManagementSystem.User.Infrastructure.Services.Validation;
using FieldManagementSystem.User.Core.Types;
using FieldManagementSystem.User.Core.Interfaces.Validation;
using Microsoft.Extensions.Logging;
using Moq;

namespace FieldManagementSystem.User.Tests;

public class UserValidationTests
{
    private Mock<ILogger<UserValidation>> _mockLogger;
    private Mock<IUserValidator> _mockUserNameValidator;
    private Mock<IUserValidator> _mockUserEmailValidator;
    private UserValidation _userValidation;
    private UserEntity _userEntity;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<UserValidation>>();
        _mockUserNameValidator = new Mock<IUserValidator>();
        _mockUserEmailValidator = new Mock<IUserValidator>();

        var validators = new List<IUserValidator> { _mockUserNameValidator.Object, _mockUserEmailValidator.Object };
        _userValidation = new UserValidation(_mockLogger.Object, validators);

        _userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _mockUserNameValidator = null;
        _mockUserEmailValidator = null;
        _userValidation = null;
        _userEntity = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Test]
    public void Validate_AllValidatorsPass_ReturnsTrueAndNullErrors()
    {
        // Arrange
        _mockUserNameValidator.Setup(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
            .Returns(true);
        _mockUserEmailValidator.Setup(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
            .Returns(true);

        // Act
        var result = _userValidation.Validate(_userEntity, out var errors);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(errors.Count(), Is.Zero);
        _mockUserNameValidator.Verify(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny), Times.Once);
        _mockUserEmailValidator.Verify(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny), Times.Once);
    }

    [Test]
    public void Validate_OneValidatorFails_ReturnsFalseAndErrors()
    {
        // Arrange
        _mockUserNameValidator.Setup(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
            .Callback((UserEntity _, ref IEnumerable<string> errs) =>
            {
                var list = errs.ToList();
                list.Add("Validator 1 error");
                errs = list;
            })
            .Returns(false);
        _mockUserEmailValidator.Setup(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
            .Returns(true);

        // Act
        var result = _userValidation.Validate(_userEntity, out var errors);
        var errorsList = errors.ToList();

        // Assert
        Assert.That(result, Is.False);
        Assert.That(errorsList, Is.Not.Null);
        Assert.That(errorsList, Does.Contain("Validator 1 error"));
        Assert.That(errorsList.Count, Is.EqualTo(1));
    }



    [Test]
    public void Validate_BothValidatorsFail_ReturnsFalseAndAllErrors()
    {
        // Arrange
        _mockUserNameValidator.Setup(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
            .Callback((UserEntity _, ref IEnumerable<string> errs) =>
            {
                var list = errs.ToList();
                list.Add("Validator 1 error");
                errs = list;
            })
            .Returns(false);
        _mockUserEmailValidator.Setup(v => v.Validate(It.IsAny<UserEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
            .Callback((UserEntity _, ref IEnumerable<string> errs) =>
            {
                var list = errs.ToList();
                list.Add("Validator 2 error");
                errs = list;
            })
            .Returns(false);

        // Act
        var result = _userValidation.Validate(_userEntity, out var errors);
        var errorsList = errors.ToList();

        // Assert
        Assert.That(result, Is.False);
        Assert.That(errorsList, Is.Not.Null);
        Assert.That(errorsList, Does.Contain("Validator 1 error"));
        Assert.That(errorsList, Does.Contain("Validator 2 error"));
        Assert.That(errorsList.Count, Is.EqualTo(2));
    }

    [Test]
    public void Validate_NoValidatorsRegistered_ReturnsTrueAndNullErrors()
    {
        // Arrange
        _userValidation = new UserValidation(_mockLogger.Object, new List<IUserValidator>()); // No validators

        // Act
        var result = _userValidation.Validate(_userEntity, out var errors);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(errors.Count(), Is.EqualTo(0));
    }
}
