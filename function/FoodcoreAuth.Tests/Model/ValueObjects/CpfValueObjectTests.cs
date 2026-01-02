using FluentAssertions;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model.ValueObjects;

namespace FoodcoreAuth.Tests.Model.ValueObjects;

public class CpfValueObjectTests
{
    [Theory]
    [InlineData("12345678909")]
    [InlineData("529.982.247-25")]
    public void Constructor_WithValidCpf_ShouldCreateInstance(string cpf)
    {
        // Act
        var cpfVo = new Cpf(cpf);

        // Assert
        cpfVo.Should().NotBeNull();
        cpfVo.Value.Should().NotBeNullOrEmpty();
        cpfVo.Value.Should().HaveLength(11);
        cpfVo.Value.Should().MatchRegex("^[0-9]+$");
    }

    [Fact]
    public void Constructor_WithFormattedCpf_ShouldStoreCleanValue()
    {
        // Arrange
        var formattedCpf = "529.982.247-25";

        // Act
        var cpfVo = new Cpf(formattedCpf);

        // Assert
        cpfVo.Value.Should().Be("52998224725");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyCpf_ShouldThrowBusinessException(string? cpf)
    {
        // Act
        Action act = () => new Cpf(cpf!);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("O CPF não pode ser nulo ou vazio.");
    }

    [Theory]
    [InlineData("12345678900")]    // Dígitos verificadores inválidos
    [InlineData("11111111111")]    // Dígitos repetidos
    [InlineData("123")]            // Muito curto
    [InlineData("1234567890123")]  // Muito longo
    public void Constructor_WithInvalidCpf_ShouldThrowBusinessException(string cpf)
    {
        // Act
        Action act = () => new Cpf(cpf);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("O CPF informado é inválido.");
    }
}
