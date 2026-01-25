using FieldManagementSystem.Services.Controller.Infrastructure.Services.Validation.Validators;
using FieldManagementSystem.Services.Controller.Core.Types;

namespace FieldManagementSystem.Services.Controller.Tests
{
    [TestFixture]
    public class ControllerNameValidatorTests
    {
        private ControllerNameValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ControllerNameValidator();
        }

        [TearDown]
        public void Teardown()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _validator = null;
        }

        [Test]
        public void Validate_ReturnsTrue_ForValidName()
        {
            // Arrange
            var entity = new ControllerEntity { Name = "Valid Name" };
            IEnumerable<string> errors = new List<string>();

            // Act
            var result = _validator.Validate(entity, ref errors);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(errors.Count(), Is.EqualTo(0));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("A")]
        public void Validate_ReturnsFalse_ForInvalidShortOrEmptyName(string name)
        {
            // Arrange
            var entity = new ControllerEntity { Name = name };
            IEnumerable<string> errors = new List<string>();

            // Act
            var result = _validator.Validate(entity, ref errors);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(errors.Count(), Is.EqualTo(1));
            Assert.That(errors.First(), Is.EqualTo("First name must be longer than 1 character."));
        }

        [Test]
        public void Validate_ReturnsFalse_ForNullName()
        {
            // Arrange
            var entity = new ControllerEntity { Name = null };
            IEnumerable<string> errors = new List<string>();

            // Act
            var result = _validator.Validate(entity, ref errors);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(errors.Count(), Is.EqualTo(1));
            Assert.That(errors.First(), Is.EqualTo("First name must be longer than 1 character."));
        }

        [Test]
        public void Validate_ReturnsFalse_ForNameWithNonEnglishLetters()
        {
            // Arrange
            var entity = new ControllerEntity { Name = "Valid Name With Numbers 123" };
            IEnumerable<string> errors = new List<string>();

            // Act
            var result = _validator.Validate(entity, ref errors);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(errors.Count(), Is.EqualTo(1));
            Assert.That(errors.First(), Is.EqualTo("First name must contain only English letters."));
        }
    }
}
