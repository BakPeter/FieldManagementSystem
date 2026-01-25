using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Core.Types;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation;
using Microsoft.Extensions.Logging;
using Moq;

namespace FieldManagementSystem.Services.Field.Tests;

[TestFixture]
public class FieldValidationTests
{
    private Mock<ILogger<FieldValidation>> _mockLogger;
    private Mock<IFieldValidator> _mockValidator1;
    private Mock<IFieldValidator> _mockValidator2;
    private FieldValidation _fieldValidation;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<FieldValidation>>();
        _mockValidator1 = new Mock<IFieldValidator>();
        _mockValidator2 = new Mock<IFieldValidator>();

        // Setup for a successful validation by default
        _mockValidator1.Setup(v => v.Validate(It.IsAny<FieldEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
                       .Returns(true);
        _mockValidator2.Setup(v => v.Validate(It.IsAny<FieldEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
                       .Returns(true);

        var validators = new List<IFieldValidator> { _mockValidator1.Object, _mockValidator2.Object };
        _fieldValidation = new FieldValidation(_mockLogger.Object, validators);
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _mockValidator1 = null;
        _mockValidator2 = null;
        _fieldValidation = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    // Validate_ReturnsTrue_WhenAllValidatorsPass
    [Test]
    public void Validate_ReturnsTrue_WhenAllValidatorsPass()
    {
        // Arrange
        var entity = new FieldEntity { Name = "Valid Name" };
        IEnumerable<string> errors;

        // Act
        var result = _fieldValidation.Validate(entity, out errors);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(errors, Is.Empty);
        _mockValidator1.Verify(v => v.Validate(entity, ref It.Ref<IEnumerable<string>>.IsAny), Times.Once);
        _mockValidator2.Verify(v => v.Validate(entity, ref It.Ref<IEnumerable<string>>.IsAny), Times.Once);
    }

    // Validate_ReturnsFalseAndCollectsErrors_WhenSomeValidatorsFail
    [Test]
    public void Validate_ReturnsFalseAndCollectsErrors_WhenSomeValidatorsFail()
    {
        // Arrange
        var entity = new FieldEntity { Name = "Invalid Name" };
        IEnumerable<string> errors;
        var error1 = "Error from Validator 1";
        var error2 = "Error from Validator 2";

        _mockValidator1.Setup(v => v.Validate(It.IsAny<FieldEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
                       .Callback(new ValidateCallback((FieldEntity e, ref IEnumerable<string> errs) =>
                       {
                           var errList = errs.ToList();
                           errList.Add(error1);
                           errs = errList;
                       }))
                       .Returns(false); // Indicates an error was added

        _mockValidator2.Setup(v => v.Validate(It.IsAny<FieldEntity>(), ref It.Ref<IEnumerable<string>>.IsAny))
                       .Callback(new ValidateCallback((FieldEntity e, ref IEnumerable<string> errs) =>
                       {
                           var errList = errs.ToList();
                           errList.Add(error2);
                           errs = errList;
                       }))
                       .Returns(false); // Indicates an error was added

        // Act
        var result = _fieldValidation.Validate(entity, out errors);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(errors.Count(), Is.EqualTo(2));
        Assert.That(errors, Does.Contain(error1));
        Assert.That(errors, Does.Contain(error2));
        _mockValidator1.Verify(v => v.Validate(entity, ref It.Ref<IEnumerable<string>>.IsAny), Times.Once);
        _mockValidator2.Verify(v => v.Validate(entity, ref It.Ref<IEnumerable<string>>.IsAny), Times.Once);
    }

    // Delegate for Callback
    private delegate void ValidateCallback(FieldEntity entity, ref IEnumerable<string> validationErrors);
}
