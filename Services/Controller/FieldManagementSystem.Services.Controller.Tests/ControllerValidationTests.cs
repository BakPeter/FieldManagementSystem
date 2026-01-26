using Moq;
using Microsoft.Extensions.Logging;
using FieldManagementSystem.Services.Controller.Infrastructure.Services.Validation;
using FieldManagementSystem.Services.Controller.Core.Types;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;

namespace FieldManagementSystem.Services.Controller.Tests
{
    [Ignore("Temporarily disabled")]
    [TestFixture]
    public class ControllerValidationTests
    {
        private Mock<ILogger<ControllerValidation>> _loggerMock;
        private ControllerValidation _validator;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ControllerValidation>>();
        }
        
        [TearDown]
        public void Teardown()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _loggerMock = null;
            _validator = null;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Validate_ReturnsTrue_WhenNoValidatorsHaveErrors()
        {
            // Arrange
            var mockValidator1 = new Mock<IContrlollerValidator>();
            var mockValidator2 = new Mock<IContrlollerValidator>();
            IEnumerable<string> errors1 = new List<string>();
            IEnumerable<string> errors2 = new List<string>();

            mockValidator1.Setup(v => v.Validate(It.IsAny<ControllerEntity>(), ref errors1)).Returns(true);
            mockValidator2.Setup(v => v.Validate(It.IsAny<ControllerEntity>(), ref errors2)).Returns(true);
            
            var validators = new[] { mockValidator1.Object, mockValidator2.Object };
            _validator = new ControllerValidation(_loggerMock.Object, validators);
            
            // Act
            var result = _validator.Validate(new ControllerEntity(), out var validationErrors);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(validationErrors.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Validate_ReturnsFalse_WhenAnyValidatorHasErrors()
        {
            // Arrange
            var mockValidator1 = new Mock<IContrlollerValidator>();
            var mockValidator2 = new Mock<IContrlollerValidator>();
            IEnumerable<string> errors1 = new List<string> { "Error 1" };
            IEnumerable<string> errors2 = new List<string>();

            mockValidator1.Setup(v => v.Validate(It.IsAny<ControllerEntity>(), ref errors1)).Returns(false);
            mockValidator2.Setup(v => v.Validate(It.IsAny<ControllerEntity>(), ref errors2)).Returns(true);
            
            var validators = new[] { mockValidator1.Object, mockValidator2.Object };
            _validator = new ControllerValidation(_loggerMock.Object, validators);

            // Act
            var result = _validator.Validate(new ControllerEntity(), out var validationErrors);

            // Assert
            var errors = validationErrors.ToList();
            Assert.That(result, Is.False);
            Assert.That(errors.Count(), Is.EqualTo(1));
            Assert.That(errors.First(), Is.EqualTo("Error 1"));
        }
    }
}
