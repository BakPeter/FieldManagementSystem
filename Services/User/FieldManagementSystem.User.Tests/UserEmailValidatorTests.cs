using FieldManagementSystem.Services.User.Core.Types;
using FieldManagementSystem.Services.User.Infrastructure.Services.Validation.Validators;

namespace FieldManagementSystem.Services.User.Tests;

public class UserEmailValidatorTests
{
    private UserEmailValidator _validator;
    private UserEntity _user;
    private IEnumerable<string> _validationErrors;

    [SetUp]
    public void Setup()
    {
        _validator = new UserEmailValidator();
        _user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        _validationErrors = new List<string>();
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    [TearDown]
    public void Teardown()
    {
        _validator = null;
        _user = null;
        _validationErrors = null;
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    [Test]
    public void Validate_ValidEmail_ReturnsTrueAndNoErrors()
    {
        // Arrange - Setup provides a valid id

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_validationErrors, Is.Empty);
    }

    [Test]
    public void Validate_InvalidEmail_MissingAtSymbol_ReturnsFalseAndError()
    {
        // Arrange
        // ReSharper disable once StringLiteralTypo
        _user.Email = "testexample.com";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email format is invalid."));
    }

    [Test]
    public void Validate_InvalidEmail_MissingDomain_ReturnsFalseAndError()
    {
        // Arrange
        _user.Email = "test@.com";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email format is invalid."));
    }

    [Test]
    public void Validate_InvalidEmail_MissingTLD_ReturnsFalseAndError()
    {
        // Arrange
        _user.Email = "test@example";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email format is invalid."));
    }

    [Test]
    public void Validate_InvalidEmail_Whitespace_ReturnsFalseAndError()
    {
        // Arrange
        _user.Email = "test@exa mple.com";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email format is invalid."));
    }

    [Test]
    public void Validate_EmailNull_ReturnsFalseAndError()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _user.Email = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email cannot be empty."));
    }

    [Test]
    public void Validate_EmailEmpty_ReturnsFalseAndError()
    {
        // Arrange
        _user.Email = string.Empty;

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email cannot be empty."));
    }

    [Test]
    public void Validate_EmailWhitespace_ReturnsFalseAndError()
    {
        // Arrange
        _user.Email = " ";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email cannot be empty."));
    }

    [Test]
    public void Validate_MultipleAtSymbols_ReturnsFalseAndError()
    {
        // Arrange
        _user.Email = "test@ex@ample.com";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Email format is invalid."));
    }

    [Test]
    public void Validate_EmailWithSpecialCharactersInName_ReturnsTrueAndNoErrors()
    {
        // Arrange
        _user.Email = "first.last+tag@example.co.uk"; // Valid special characters in local part

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_validationErrors, Is.Empty);
    }
}
