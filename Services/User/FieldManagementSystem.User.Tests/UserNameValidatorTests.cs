using FieldManagementSystem.User.Infrastructure.Services.Validation.Validators;
using FieldManagementSystem.User.Core.Types;

namespace FieldManagementSystem.User.Tests;

public class UserNameValidatorTests
{
    private UserNameValidator _validator;
    private UserEntity _user;
    private IEnumerable<string> _validationErrors;

    [SetUp]
    public void Setup()
    {
        _validator = new UserNameValidator();
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
    public void Validate_ValidNames_ReturnsTrueAndNoErrors()
    {
        // Arrange - Setup provides valid names

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_validationErrors, Is.Empty);
    }

    [Test]
    public void Validate_FirstNameTooShort_ReturnsFalseAndError()
    {
        // Arrange
        _user.FirstName = "J";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("First name must be longer than 1 character."));
    }

    [Test]
    public void Validate_LastNameTooShort_ReturnsFalseAndError()
    {
        // Arrange
        _user.LastName = "D";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Last name must be longer than 1 character."));
    }

    [Test]
    public void Validate_FirstNameContainsNonEnglishLetters_ReturnsFalseAndError()
    {
        // Arrange
        _user.FirstName = "J1hn";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("First name must contain only English letters."));
    }

    [Test]
    public void Validate_LastNameContainsNonEnglishLetters_ReturnsFalseAndError()
    {
        // Arrange
        _user.LastName = "Doè"; // 'è' is not an English letter

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Last name must contain only English letters."));
    }

    [Test]
    public void Validate_FirstNameNull_ReturnsFalseAndError()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _user.FirstName = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("First name must be longer than 1 character."));
    }

    [Test]
    public void Validate_FirstNameEmpty_ReturnsFalseAndError()
    {
        // Arrange
        _user.FirstName = string.Empty;

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("First name must be longer than 1 character."));
    }

    [Test]
    public void Validate_FirstNameWhitespace_ReturnsFalseAndError()
    {
        // Arrange
        _user.FirstName = " ";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("First name must be longer than 1 character."));
    }

    [Test]
    public void Validate_LastNameNull_ReturnsFalseAndError()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _user.LastName = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Last name must be longer than 1 character."));
    }

    [Test]
    public void Validate_LastNameEmpty_ReturnsFalseAndError()
    {
        // Arrange
        _user.LastName = string.Empty;

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Last name must be longer than 1 character."));
    }

    [Test]
    public void Validate_LastNameWhitespace_ReturnsFalseAndError()
    {
        // Arrange
        _user.LastName = " ";

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_validationErrors.ToList(), Does.Contain("Last name must be longer than 1 character."));
    }

    [Test]
    public void Validate_MultipleErrors_ReturnsFalseAndAllErrors()
    {
        // Arrange
        _user.FirstName = "J1"; // Invalid character
        _user.LastName = "D"; // Too short

        // Act
        var result = _validator.Validate(_user, ref _validationErrors);
        var errors = _validationErrors.ToList();

        // Assert
        Assert.That(result, Is.False);
        Assert.That(errors, Does.Contain("First name must contain only English letters."));
        Assert.That(errors, Does.Contain("Last name must be longer than 1 character."));
        Assert.That(errors.Count(), Is.EqualTo(2));
    }
}
