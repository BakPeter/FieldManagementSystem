using Moq;
using Microsoft.Extensions.Logging;
using FieldManagementSystem.Services.Controller.Infrastructure.Services;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Controller.Core.Types.DTOs;
using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Tests
{
    [TestFixture]
    public class ControllerServiceTests
    {
        private Mock<ILogger<ControllerService>> _loggerMock;
        private Mock<IControllerRepository> _repositoryMock;
        private Mock<IControllerValidation> _validationMock;
        private ControllerService _service;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ControllerService>>();
            _repositoryMock = new Mock<IControllerRepository>();
            _validationMock = new Mock<IControllerValidation>();
            _service = new ControllerService(_loggerMock.Object, _repositoryMock.Object, _validationMock.Object);
        }

        [TearDown]
        public void Teardown()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

            _loggerMock = null;
            _repositoryMock = null;
            _validationMock = null;
            _service = null;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public async Task GetControllersAsync_ReturnsAllControllers()
        {
            // Arrange
            var controllers = new List<ControllerEntity> { new ControllerEntity(), new ControllerEntity() };
            _repositoryMock.Setup(r => r.GetAllControllersAsync()).ReturnsAsync(controllers);

            // Act
            var result = await _service.GetControllersAsync();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(((List<ControllerEntity>)result.Data).Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetControllerAsync_ReturnsController_WhenFound()
        {
            // Arrange
            var controllerId = Guid.NewGuid().ToString();
            var controller = new ControllerEntity { Id = new Guid(controllerId) };
            _repositoryMock.Setup(r => r.GetControllerAsync(controllerId)).ReturnsAsync(controller);

            // Act
            var result = await _service.GetControllerAsync(controllerId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data.Id, Is.EqualTo(new Guid(controllerId)));
        }

        [Test]
        public async Task GetControllerAsync_ReturnsError_WhenNotFound()
        {
            // Arrange
            var controllerId = "test-id";
            _repositoryMock.Setup(r => r.GetControllerAsync(controllerId)).ReturnsAsync((ControllerEntity)null);

            // Act
            var result = await _service.GetControllerAsync(controllerId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public async Task CreateControllerAsync_ReturnsCreatedController_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateControllerDto();
            var validationErrors = new List<string>();
            _validationMock.Setup(v => v.Validate(It.IsAny<ControllerEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
            _repositoryMock.Setup(r => r.GetControllerByNameAsync(It.IsAny<string>())).ReturnsAsync(null as ControllerEntity);
            _repositoryMock.Setup(r => r.CreateControllerAsync(It.IsAny<ControllerEntity>())).ReturnsAsync(true);

            // Act
            var result = await _service.CreateControllerAsync(createDto);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
        }

        [Test]
        public async Task UpdateController_ReturnsSuccessMessage_WhenSuccessful()
        {
            // Arrange
            var updateDto = new UpdateControllerDto { Id = Guid.NewGuid() };
            var existingController = new ControllerEntity { Id = updateDto.Id };
            _repositoryMock.Setup(r => r.GetControllerAsync(updateDto.Id.ToString())).ReturnsAsync(existingController);
            List<string> validationErrors = new List<string>();
            _validationMock.Setup(v => v.Validate(It.IsAny<ControllerEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
            _repositoryMock.Setup(r => r.UpdateController(It.IsAny<ControllerEntity>())).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateController(updateDto);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Does.Contain("updated"));
        }

        [Test]
        public async Task DeleteControllerAsync_ReturnsSuccessMessage_WhenSuccessful()
        {
            // Arrange
            var controllerId = Guid.NewGuid().ToString();
            var existingController = new ControllerEntity { Id = new Guid(controllerId) };
            _repositoryMock.Setup(r => r.GetControllerAsync(controllerId)).ReturnsAsync(existingController);
            _repositoryMock.Setup(r => r.DeleteController(controllerId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteControllerAsync(controllerId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Does.Contain("deleted"));
        }
    }
}
