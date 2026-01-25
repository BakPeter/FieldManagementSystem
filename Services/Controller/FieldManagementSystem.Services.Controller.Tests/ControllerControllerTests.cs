using Moq;
using Microsoft.Extensions.Logging;
using FieldManagementSystem.Services.Controller.Controllers;
using FieldManagementSystem.Services.Controller.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FieldManagementSystem.Services.Controller.Core.Types;
using FieldManagementSystem.Services.Controller.Core.Types.DTOs;

namespace FieldManagementSystem.Services.Controller.Tests
{
    [Ignore("Temporarily disabled")]
    [TestFixture]
    public class ControllerControllerTests
    {
        private Mock<ILogger<ControllerController>> _loggerMock;
        private Mock<IControllerService> _serviceMock;
        private ControllerController _controller;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ControllerController>>();
            _serviceMock = new Mock<IControllerService>();
            _controller = new ControllerController(_loggerMock.Object, _serviceMock.Object);
        }
        
        [TearDown]
        public void Teardown()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _loggerMock = null;
            _serviceMock = null;
            _controller = null;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

        }

        [Test]
        public async Task GetControllers_ReturnsOkResult_WithListOfControllers()
        {
            // Arrange
            var controllers = new List<ControllerEntity> { new ControllerEntity(), new ControllerEntity() };
            var serviceResponse = new Result<IEnumerable<ControllerEntity>>(true, controllers);
            _serviceMock.Setup(s => s.GetControllersAsync()).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetController();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<ControllerEntity>>());
            var returnValue = (IEnumerable<ControllerEntity>)okResult.Value;
            Assert.That(returnValue.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetControllers_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var error = new Exception("Service Failure");
            var serviceResponse = new Result<IEnumerable<ControllerEntity>>(false, null, error);
            _serviceMock.Setup(s => s.GetControllersAsync()).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetController();

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(badRequestResult.Value, Is.EqualTo("Service Failure"));
        }

        [Test]
        public async Task GetControllerById_ReturnsOkResult_WithController()
        {
            // Arrange
            var controllerId = Guid.NewGuid().ToString(); // Generate a valid GUID
            var controller = new ControllerEntity { Id = new Guid(controllerId) };
            var serviceResponse = new Result<ControllerEntity>(true, controller);
            _serviceMock.Setup(s => s.GetControllerAsync(controllerId)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetController(controllerId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.InstanceOf<ControllerEntity>());
            var returnValue = (ControllerEntity)okResult.Value;
            Assert.That(returnValue.Id.ToString(), Is.EqualTo(controllerId));
        }

        [Test]
        public async Task GetControllerById_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var controllerId = Guid.NewGuid().ToString(); // Generate a valid GUID
            var error = new Exception("Service Failure");
            var serviceResponse = new Result<ControllerEntity>(false, null, error);
            _serviceMock.Setup(s => s.GetControllerAsync(controllerId)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetController(controllerId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(badRequestResult.Value, Is.EqualTo("Service Failure"));
        }

        [Test]
        public async Task CreateController_ReturnsCreatedAtAction_WithController()
        {
            // Arrange
            var createDto = new CreateControllerDto();
            var controller = new ControllerEntity { Id = Guid.NewGuid() };
            var serviceResponse = new Result<ControllerEntity>(true, controller);
            _serviceMock.Setup(s => s.CreateControllerAsync(createDto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateController(createDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtActionResult = (CreatedAtActionResult)result;
            Assert.That(createdAtActionResult.Value, Is.InstanceOf<ControllerEntity>());
            var returnValue = (ControllerEntity)createdAtActionResult.Value;
            Assert.That(returnValue.Id, Is.EqualTo(controller.Id));
        }

        [Test]
        public async Task UpdateController_ReturnsOkResult_WithUpdatedController()
        {
            // Arrange
            var updateDto = new UpdateControllerDto();
            var serviceResponse = new Result<string>(true, "Controller updated successfully");
            _serviceMock.Setup(s => s.UpdateController(updateDto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.UpdateController(updateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo("Controller updated successfully"));
        }

        [Test]
        public async Task DeleteController_ReturnsOkResult_WithConfirmation()
        {
            // Arrange
            var controllerId = Guid.NewGuid().ToString(); // Generate a valid GUID
            var serviceResponse = new Result<string>(true, "Controller deleted successfully");
            _serviceMock.Setup(s => s.DeleteControllerAsync(controllerId)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.DeleteController(controllerId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo("Controller deleted successfully"));
        }
    }
}
