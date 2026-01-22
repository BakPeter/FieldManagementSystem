using FieldManagementSystem.Services.Repository.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Repository.Core.Types;
using FieldManagementSystem.Services.Repository.Infrastructure.Services;
using FieldManagementSystem.Services.Tests.RepositoryTests.Types;
using Moq;

namespace FieldManagementSystem.Services.Tests.RepositoryTests;

public class EntityValidationTests
{
    private Mock<IEntityValidator<TestEntity>> _validator1Mock = null!;
    private Mock<IEntityValidator<TestEntity>> _validator2Mock = null!;
    private EntityValidation<TestEntity> _entityValidation = null!;
    private TestEntity _entity = null!;

    [SetUp]
    public void Setup()
    {
        _validator1Mock = new Mock<IEntityValidator<TestEntity>>();
        _validator2Mock = new Mock<IEntityValidator<TestEntity>>();
        _entity = new TestEntity { Id = 1, Name = "Alpha" };
    }

    [TearDown]
    public void TearDown()
    {
        _validator1Mock = null!;
        _validator2Mock = null!;
        _entityValidation = null!;
        _entity = null!;
    }

    [Test]
    public void Validate_EntityValidAllValidatorsPass_ReturnsValidResult()
    {
        // Arrange
        _validator1Mock.Setup(v => v.Validate(_entity))
            .Returns(new ValidationResult(true));
        _validator2Mock.Setup(v => v.Validate(_entity))
            .Returns(new ValidationResult(true));

        _entityValidation = new EntityValidation<TestEntity>([_validator1Mock.Object, _validator2Mock.Object]);

        // Act
        var result = _entityValidation.Validate(_entity);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ValidationErrors, Is.Null);
    }

    [Test]
    public void Validate_OneValidatorFails_ReturnsInvalidWithError()
    {
        // Arrange
        var validationErrors = new List<string> { "Name is required" };

        _validator1Mock.Setup(v => v.Validate(_entity))
            .Returns(new ValidationResult(true));
        _validator2Mock.Setup(v => v.Validate(_entity))
            .Returns(new ValidationResult(false, validationErrors));

        _entityValidation = new EntityValidation<TestEntity>([_validator1Mock.Object, _validator2Mock.Object]);

        // Act
        var result = _entityValidation.Validate(_entity);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors, Is.EquivalentTo(validationErrors));
    }

    [Test]
    public void Validate_MultipleValidatorsFail_ReturnsInValidAndAggregatedErrors()
    {
        // Arrange
        var errors1 = new List<string> { "Name is required" };
        var errors2 = new List<string> { "Id must be positive" };

        _validator1Mock.Setup(v => v.Validate(_entity))
            .Returns(new ValidationResult(false, errors1));
        _validator2Mock.Setup(v => v.Validate(_entity))
            .Returns(new ValidationResult(false, errors2));

        _entityValidation = new EntityValidation<TestEntity>([_validator1Mock.Object, _validator2Mock.Object]);

        // Act
        var result = _entityValidation.Validate(_entity);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ValidationErrors, Is.EquivalentTo(new[] { "Name is required", "Id must be positive" }));
    }


    [Test]
    public void Validate_NoValidators_ReturnsValid()
    {
        // Arrange
        _entityValidation = new EntityValidation<TestEntity>(new List<IEntityValidator<TestEntity>>());

        // Act
        var result = _entityValidation.Validate(_entity);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ValidationErrors, Is.Null);
    }
}