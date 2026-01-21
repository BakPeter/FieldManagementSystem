using Moq;
using Repository.Core.Interfaces;
using Repository.Core.Interfaces.Validation;
using Repository.Core.Types;
using Repository.Infrastructure.Services;
using RepositoryUnitTests.Types;

namespace RepositoryUnitTests;


[TestFixture]
public class RepositoryTests
{
    private Mock<IRepositoryAdapter<TestEntity>> _repositoryAdapterMock = null!;
    private Mock<IEntityValidation<TestEntity>> _validationMock = null!;
    private Repository<TestEntity> _repository = null!;

    [SetUp]
    public void Setup()
    {
        _repositoryAdapterMock = new Mock<IRepositoryAdapter<TestEntity>>();
        _validationMock = new Mock<IEntityValidation<TestEntity>>();
        _repository = new Repository<TestEntity>(_repositoryAdapterMock.Object, _validationMock.Object);
    }

    // [TearDown]
    // public void Teardown()
    // {
    //     _repositoryAdapterMock = null;
    //     _validationMock = null;
    //     _repository = null;
    // }

    private static bool TestEntityComparer(TestEntity? a, TestEntity? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;

        var nestedEqual = (a.Nested == null && b.Nested == null) ||
                           (a.Nested != null && b.Nested != null &&
                            a.Nested.Key == b.Nested.Key &&
                            a.Nested.Value == b.Nested.Value);

        var nestedListEqual = a.NestedList.Count == b.NestedList.Count &&
                               a.NestedList.Zip(b.NestedList, (x, y) => x.Key == y.Key && x.Value == y.Value)
                                   .All(x => x);

        return a.Id == b.Id &&
               a.Name == b.Name &&
               a.IsActive == b.IsActive &&
               a.Status == b.Status &&
               a.Values.SequenceEqual(b.Values) &&
               nestedEqual &&
               nestedListEqual;
    }

    [Test]
    public async Task GetByIdAsync_WhenFound_ShouldReturnResultWithEntity()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = 1,
            Name = "John",
            Status = StatusType.Active,
            Nested = new SubEntity { Key = "A", Value = 10 },
            NestedList = [new SubEntity { Key = "B", Value = 20 }]
        };

        // Act
        _repositoryAdapterMock.Setup(a => a.GetByIdAsync(1))
            .ReturnsAsync(new Result<TestEntity>(true, entity));

        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(result.Entity, Is.EqualTo(entity).Using<TestEntity>(TestEntityComparer));
    }

    [Test]
    public async Task GetByIdAsync_WhenNotFound_ShouldReturnEmptyResult()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = 1,
            Name = "John",
            Status = StatusType.Active,
            Nested = new SubEntity { Key = "A", Value = 10 },
            NestedList = [new SubEntity { Key = "B", Value = 20 }]
        };
        var expectedException = new EntityExistsException();

        _repositoryAdapterMock.Setup(a => a.GetByIdAsync(1))
            .ReturnsAsync(new Result<TestEntity>(false, entity, expectedException));

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo(expectedException));
        Assert.That(result.Entity, Is.EqualTo(entity).Using<TestEntity>(TestEntityComparer));
    }

    [Test]
    public async Task GetAllAsync_WhenInvoked_ShouldReturnAllEntities()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = 1, Status = StatusType.Active },
            new TestEntity { Id = 2, Status = StatusType.Inactive }
        };

        _repositoryAdapterMock.Setup(a => a.GetAllAsync())
            .ReturnsAsync(new Result<IEnumerable<TestEntity>>(true, entities));

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Entity, Is.EquivalentTo(entities)
            .Using<TestEntity>(TestEntityComparer));
    }


    [Test]
    public async Task AddAsync_WhenEntityInValid_ShouldReturnFailedResultAndInValidException()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "@In valid Name@" };
        var validationErrors = new List<string> { "Invalid name" };
        var validationResult = new ValidationResult(false, validationErrors);
        _validationMock.Setup(v => v.Validate(entity)).Returns(validationResult);

        // Act
        var result = await _repository.AddAsync(entity);
        var error = result.Error as InValidEntityException;

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(error, Is.TypeOf<InValidEntityException>());
    }

    [Test]
    public async Task AddAsync_WhenEntityExists_ShouldReturnFail()
    {
        // Arrange
        var entity = new TestEntity { Id = 1 };
        _validationMock.Setup(v => v.Validate(entity)).Returns(new ValidationResult(true));
        _repositoryAdapterMock.Setup(a => a.Contains(entity)).ReturnsAsync(true);

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.TypeOf<EntityExistsException>());
    }

    [Test]
    public async Task AddAsync_WhenEntityIsValidAndNotExists_ShouldReturnSuccess()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = 1,
            Name = "OK",
            Status = StatusType.Pending,
            Nested = new SubEntity { Key = "X", Value = 100 },
            NestedList = [new SubEntity { Key = "Y", Value = 200 }]
        };
        var expectedResult = new Result<TestEntity>(true, entity);

        _validationMock.Setup(v => v.Validate(entity)).Returns(new ValidationResult(true));
        _repositoryAdapterMock.Setup(a => a.Contains(entity)).ReturnsAsync(false);
        _repositoryAdapterMock.Setup(a => a.AddAsync(entity)).ReturnsAsync(expectedResult);

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Entity, Is.EqualTo(entity).Using<TestEntity>(TestEntityComparer));
    }

    [Test]
    public async Task UpdateAsync_WhenEntityNotExists_ShouldFail()
    {
        // Arrange
        var entity = new TestEntity { Id = 1 };
        var error = new EntityDontExistsException();
        var expectedResult = new Result<TestEntity>(false, entity, error);

        _repositoryAdapterMock.Setup(a => a.Contains(entity)).ReturnsAsync(false);
        _repositoryAdapterMock.Setup(a => a.UpdateAsync(entity)).ReturnsAsync(expectedResult);


        // Act
        var result = await _repository.UpdateAsync(entity);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.TypeOf<EntityDontExistsException>());
        Assert.That(result.Entity, Is.EqualTo(entity).Using<TestEntity>(TestEntityComparer));
    }

    [Test]
    public async Task UpdateAsync_WhenEntityExists_ShouldSuccess()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = 1,
            Status = StatusType.Active,
            Nested = new SubEntity { Key = "N", Value = 1 },
            NestedList = [new SubEntity { Key = "L", Value = 2 }]
        };
        var expectedResult = new Result<TestEntity>(true, entity);

        _repositoryAdapterMock.Setup(a => a.Contains(entity)).ReturnsAsync(true);
        _repositoryAdapterMock.Setup(a => a.UpdateAsync(entity)).ReturnsAsync(expectedResult);

        // Act
        var result = await _repository.UpdateAsync(entity);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(result.Entity, Is.EqualTo(entity).Using<TestEntity>(TestEntityComparer));
    }


    [Test]
    public async Task DeleteAsync_WhenEntityExists_ShouldSuccess()
    {
        // Arrange
        var entity = new TestEntity { Id = 1 };
        var expectedResult = new Result<TestEntity>(true, entity);

        _repositoryAdapterMock.Setup(a => a.Contains(entity)).ReturnsAsync(true);
        _repositoryAdapterMock.Setup(a => a.DeleteAsync(entity)).ReturnsAsync(expectedResult);

        // Act
        var result = await _repository.DeleteAsync(entity);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(result.Entity, Is.EqualTo(entity).Using<TestEntity>(TestEntityComparer));
    }

    [Test]
    public async Task DeleteAsync_WhenNotExists_ShouldFail()
    {
        // Arrange
        var entity = new TestEntity { Id = 1 };
        var error = new EntityDontExistsException();
        var expectedResult = new Result<TestEntity>(false, entity, error);

        _repositoryAdapterMock.Setup(a => a.Contains(entity)).ReturnsAsync(false);
        _repositoryAdapterMock.Setup(a => a.DeleteAsync(entity)).ReturnsAsync(expectedResult);

        // Act
        var result = await _repository.DeleteAsync(entity);
       
        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.TypeOf<EntityDontExistsException>());
        Assert.That(result.Entity, Is.EqualTo(entity).Using<TestEntity>(TestEntityComparer));
    }
    //     [Test]
    //     public void MethodName_Scenario_ExpectedBehaviour()
    //     {
    //         // Arrange
    //         // Act
    //         // Assert
    //     }
}