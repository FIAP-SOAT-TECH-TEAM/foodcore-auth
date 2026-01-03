using FluentAssertions;
using Foodcore.Auth.Helpers.Validation;

namespace FoodcoreAuth.Tests.Helpers.Validation;

public class CpfAttributeTests
{
    private readonly CpfAttribute _validator = new();

    #region Valid CPF Tests

    [Fact]
    public void IsValid_WithValidCpf_ShouldReturnTrue()
    {
        // Arrange
        var cpf = "12345678909";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithNullValue_ShouldReturnTrue()
    {
        // Arrange
        object? cpf = null;

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithEmptyString_ShouldReturnTrue()
    {
        // Arrange
        var cpf = "";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithWhitespaceString_ShouldReturnTrue()
    {
        // Arrange
        var cpf = "   ";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Invalid CPF Tests

    [Fact]
    public void IsValid_WithInvalidCpf_ShouldReturnFalse()
    {
        // Arrange
        var cpf = "12345678900";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithAllSameDigits_ShouldReturnFalse()
    {
        // Arrange
        var cpf = "11111111111";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithShortCpf_ShouldReturnFalse()
    {
        // Arrange
        var cpf = "123456789";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithLongCpf_ShouldReturnFalse()
    {
        // Arrange
        var cpf = "123456789012";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithLetters_ShouldReturnFalse()
    {
        // Arrange
        var cpf = "1234567890a";

        // Act
        var result = _validator.IsValid(cpf);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Attribute Properties Tests

    [Fact]
    public void CpfAttribute_ShouldBeOfTypeValidationAttribute()
    {
        // Assert
        _validator.Should().BeAssignableTo<System.ComponentModel.DataAnnotations.ValidationAttribute>();
    }

    #endregion
}

public class NullableEmailAddressAttributeTests
{
    private readonly NullableEmailAddressAttribute _validator = new();

    #region Valid Email Tests

    [Fact]
    public void IsValid_WithValidEmail_ShouldReturnTrue()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithNullValue_ShouldReturnTrue()
    {
        // Arrange
        object? email = null;

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithEmptyString_ShouldReturnTrue()
    {
        // Arrange
        var email = "";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithWhitespaceString_ShouldReturnTrue()
    {
        // Arrange
        var email = "   ";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithComplexEmail_ShouldReturnTrue()
    {
        // Arrange
        var email = "user.name+tag@subdomain.example.com";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Invalid Email Tests

    [Fact]
    public void IsValid_WithInvalidEmail_ShouldReturnFalse()
    {
        // Arrange
        var email = "invalid-email";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithEmailMissingAt_ShouldReturnFalse()
    {
        // Arrange
        var email = "testexample.com";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithEmailMissingDomain_ShouldReturnFalse()
    {
        // Arrange
        var email = "test@";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithOnlyAt_ShouldReturnFalse()
    {
        // Arrange
        var email = "@";

        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Attribute Properties Tests

    [Fact]
    public void NullableEmailAddressAttribute_ShouldBeOfTypeValidationAttribute()
    {
        // Assert
        _validator.Should().BeAssignableTo<System.ComponentModel.DataAnnotations.ValidationAttribute>();
    }

    #endregion
}
