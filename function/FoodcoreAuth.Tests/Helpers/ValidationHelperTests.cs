using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Foodcore.Auth.Helpers.Validation;

namespace FoodcoreAuth.Tests.Helpers;

public class ValidationHelperTests
{
    private class TestDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string? Email { get; set; }

        [StringLength(10, MinimumLength = 3, ErrorMessage = "O campo deve ter entre 3 e 10 caracteres.")]
        public string? Code { get; set; }
    }

    [Fact]
    public void Validate_WithValidObject_ShouldNotThrow()
    {
        // Arrange
        var dto = new TestDto
        {
            Name = "Test Name",
            Email = "test@example.com",
            Code = "ABC123"
        };

        // Act
        Action act = () => ValidationHelper.Validate(dto);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithNullObject_ShouldNotThrow()
    {
        // Act
        Action act = () => ValidationHelper.Validate<TestDto>(null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithMissingRequiredField_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new TestDto
        {
            Name = null, // Required field
            Email = "test@example.com"
        };

        // Act
        Action act = () => ValidationHelper.Validate(dto);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("O nome é obrigatório.");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new TestDto
        {
            Name = "Test Name",
            Email = "invalid-email"
        };

        // Act
        Action act = () => ValidationHelper.Validate(dto);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Email inválido.");
    }

    [Fact]
    public void Validate_WithInvalidStringLength_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new TestDto
        {
            Name = "Test Name",
            Code = "AB" // Menos que 3 caracteres
        };

        // Act
        Action act = () => ValidationHelper.Validate(dto);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("O campo deve ter entre 3 e 10 caracteres.");
    }
}
