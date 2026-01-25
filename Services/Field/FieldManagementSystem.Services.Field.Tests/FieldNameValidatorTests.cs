using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation.Validators;

namespace FieldManagementSystem.Services.Field.Tests;

[TestFixture]
public class FieldNameValidatorTests
{
    private FieldNameValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new FieldNameValidator();
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _validator = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    // Validate_ReturnsTrue_WhenNameIsValid
    [Test]
    public void Validate_ReturnsTrue_WhenNameIsValid()
    {
        // Arrange
        var entity = new FieldEntity { Name = "Valid Field Name" };
        IEnumerable<string> validationErrors = new List<string>();

        // Act
        var result = _validator.Validate(entity, ref validationErrors);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }

    // Validate_ReturnsFalse_WhenNameIsNullOrEmpty
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Validate_ReturnsFalse_WhenNameIsNullOrEmpty(string? invalidName)
    {
        // Arrange
        var entity = new FieldEntity { Name = invalidName };
        IEnumerable<string> validationErrors = new List<string>();

        // Act
        var result = _validator.Validate(entity, ref validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(validationErrors.Single(), Is.EqualTo("Field Name must be longer than 1 character."));
    }

    // Validate_ReturnsFalse_WhenNameIsTooShort
    [Test]
    public void Validate_ReturnsFalse_WhenNameIsTooShort()
    {
        // Arrange
        var entity = new FieldEntity { Name = "A" };
        IEnumerable<string> validationErrors = new List<string>();

        // Act
        var result = _validator.Validate(entity, ref validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(validationErrors.Single(), Is.EqualTo("Field Name must be longer than 1 character."));
    }

    // Validate_ReturnsFalse_WhenNameContainsInvalidCharacters
    [TestCase("Field1")]
    [TestCase("Field!")]
    [TestCase("Field#Name")]
    [TestCase("Field_Name")]
    public void Validate_ReturnsFalse_WhenNameContainsInvalidCharacters(string invalidName)
    {
        // Arrange
        var entity = new FieldEntity { Name = invalidName };
        IEnumerable<string> validationErrors = new List<string>();

        // Act
        var result = _validator.Validate(entity, ref validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(validationErrors.Single(), Is.EqualTo("Field Name must contain only English letters and ' '."));
    }

    // Validate_AppendsErrors_WhenExistingErrorsArePresent
    [Test]
    public void Validate_AppendsErrors_WhenExistingErrorsArePresent()
    {
        // Arrange
        var entity = new FieldEntity { Name = "F" }; // Invalid name
        IEnumerable<string> validationErrors = new List<string> { "Existing Error 1" };
        var initialErrorCount = validationErrors.Count();

        // Act
        var result = _validator.Validate(entity, ref validationErrors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(validationErrors.Count(), Is.EqualTo(initialErrorCount + 1));
        Assert.That(validationErrors.First(), Is.EqualTo("Existing Error 1"));
        Assert.That(validationErrors.Last(), Is.EqualTo("Field Name must be longer than 1 character."));
    }
}
