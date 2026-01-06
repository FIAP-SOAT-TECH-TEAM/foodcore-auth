using FluentAssertions;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model.ValueObjects;

namespace FoodcoreAuth.Tests.Model.ValueObjects;

public class PasswordValueObjectTests
{
    [Theory]
    [InlineData("Password1!")]
    [InlineData("Str0ng@Pass")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("Test#123abc")]
    public void Constructor_WithValidPassword_ShouldCreateInstance(string password)
    {
        // Act
        var passwordVo = new Password(password);

        // Assert
        passwordVo.Should().NotBeNull();
        passwordVo.Value.Should().Be(password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyPassword_ShouldThrowBusinessException(string? password)
    {
        // Act
        Action act = () => new Password(password!);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("A senha não pode ser nula ou vazia.");
    }

    [Theory]
    [InlineData("Short1!")]     // Muito curta (7 caracteres)
    [InlineData("Ab1!")]        // Muito curta (4 caracteres)
    public void Constructor_WithTooShortPassword_ShouldThrowBusinessException(string password)
    {
        // Act
        Action act = () => new Password(password);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("A senha deve ter entre 8 e 20 caracteres.");
    }

    [Fact]
    public void Constructor_WithTooLongPassword_ShouldThrowBusinessException()
    {
        // Arrange
        var password = "ThisIsAVeryLongP@ss1"; // 21 caracteres
        var longPassword = password + "x";

        // Act
        Action act = () => new Password(longPassword);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("A senha deve ter entre 8 e 20 caracteres.");
    }

    [Theory]
    [InlineData("password1!")]    // Sem maiúscula
    [InlineData("PASSWORD1!")]    // Sem minúscula
    [InlineData("Password!!")]    // Sem número
    [InlineData("Password12")]    // Sem caractere especial
    public void Constructor_WithMissingRequirement_ShouldThrowBusinessException(string password)
    {
        // Act
        Action act = () => new Password(password);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.");
    }

    [Fact]
    public void Constructor_WithExactMinLength_ShouldCreateInstance()
    {
        // Arrange - 8 caracteres com todos os requisitos
        var password = "Abcd12#$";

        // Act
        var passwordVo = new Password(password);

        // Assert
        passwordVo.Value.Should().Be(password);
    }

    [Fact]
    public void Constructor_WithExactMaxLength_ShouldCreateInstance()
    {
        // Arrange - 20 caracteres com todos os requisitos
        var password = "Abcd12#$Abcd12#$Abcd";

        // Act
        var passwordVo = new Password(password);

        // Assert
        passwordVo.Value.Should().Be(password);
    }
}
