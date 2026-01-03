using FluentAssertions;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model.ValueObjects;

namespace FoodcoreAuth.Tests.Model.ValueObjects;

public class EmailValueObjectTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user@domain.org")]
    [InlineData("user.name@company.com.br")]
    [InlineData("test+tag@gmail.com")]
    public void Constructor_WithValidEmail_ShouldCreateInstance(string email)
    {
        // Act
        var emailVo = new Email(email);

        // Assert
        emailVo.Should().NotBeNull();
        emailVo.Value.Should().Be(email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyEmail_ShouldThrowBusinessException(string? email)
    {
        // Act
        Action act = () => new Email(email!);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("O Email informado é inválido.");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("no-at-sign.com")]
    [InlineData("missing")]
    public void Constructor_WithEmailWithoutAtSign_ShouldThrowBusinessException(string email)
    {
        // Act
        Action act = () => new Email(email);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("O Email informado é inválido.");
    }
}
