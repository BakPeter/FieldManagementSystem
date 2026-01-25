using Moq;
using Microsoft.Extensions.Logging;
using FieldManagementSystem.Services.Controller.Infrastructure.Services.Repository;
using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Tests
{
    [TestFixture]
    public class ControllerCachedRepositoryAdapterTests
    {
        private Mock<ILogger<ControllerCachedRepositoryAdapter>> _loggerMock;
        private ControllerCachedRepositoryAdapter _adapter;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ControllerCachedRepositoryAdapter>>();
            _adapter = new ControllerCachedRepositoryAdapter(_loggerMock.Object);
        }

        [TearDown]
        public void Teardown()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _loggerMock = null;
            _adapter = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public async Task GetAllControllersAsync_ReturnsAllControllers()
        {
            // Act
            var result = await _adapter.GetAllControllersAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetControllerAsync_ReturnsController_WhenFound()
        {
            // Arrange
            var existingController = (await _adapter.GetAllControllersAsync()).First();

            // Act
            var result = await _adapter.GetControllerAsync(existingController.Id.ToString());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(existingController.Id));
        }

        [Test]
        public async Task GetControllerAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _adapter.GetControllerAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateControllerAsync_AddsController()
        {
            // Arrange
            var newController = new ControllerEntity { Id = Guid.NewGuid(), Name = "New Controller" };

            // Act
            var success = await _adapter.CreateControllerAsync(newController);
            var result = await _adapter.GetControllerAsync(newController.Id.ToString());

            // Assert
            Assert.That(success, Is.True);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("New Controller"));
        }

        [Test]
        public async Task UpdateController_UpdatesExistingController()
        {
            // Arrange
            var existingController = (await _adapter.GetAllControllersAsync()).First();
            existingController.Name = "Updated Name";

            // Act
            var success = await _adapter.UpdateController(existingController);
            var result = await _adapter.GetControllerAsync(existingController.Id.ToString());

            // Assert
            Assert.That(success, Is.True);
            Assert.That(result.Name, Is.EqualTo("Updated Name"));
        }

        [Test]
        public async Task DeleteController_RemovesController()
        {
            // Arrange
            var existingController = (await _adapter.GetAllControllersAsync()).First();

            // Act
            var success = await _adapter.DeleteController(existingController.Id.ToString());
            var result = await _adapter.GetControllerAsync(existingController.Id.ToString());

            // Assert
            Assert.That(success, Is.True);
            Assert.That(result, Is.Null);
        }
    }
}
