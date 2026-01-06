using FluentAssertions;
using Foodcore.Auth.Utils;

namespace FoodcoreAuth.Tests.Utils;

public class CpfUtilsTests
{
    [Theory]
    [InlineData("12345678909", true)]   // CPF válido
    [InlineData("529.982.247-25", true)] // CPF válido com formatação
    [InlineData("11111111111", false)]  // CPF com todos os dígitos iguais
    [InlineData("00000000000", false)]  // CPF com todos zeros
    [InlineData("12345678900", false)]  // CPF inválido (dígitos verificadores errados)
    [InlineData("123", false)]          // CPF muito curto
    [InlineData("1234567890123", false)] // CPF muito longo
    public void IsCpf_ShouldValidateCpfCorrectly(string cpf, bool expectedResult)
    {
        // Act
        var result = CpfUtils.IsCpf(cpf);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123.456.789-09", "12345678909")]
    [InlineData("12345678909", "12345678909")]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("  123.456.789-09  ", "12345678909")]
    public void CleanCpf_ShouldRemoveFormattingCharacters(string input, string expected)
    {
        // Act
        var result = CpfUtils.CleanCpf(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsCpf_WithRepeatedDigits_ShouldReturnFalse()
    {
        // Arrange
        var repeatedDigitsCpfs = new[]
        {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
        };

        // Act & Assert
        foreach (var cpf in repeatedDigitsCpfs)
        {
            CpfUtils.IsCpf(cpf).Should().BeFalse($"CPF {cpf} com dígitos repetidos deveria ser inválido");
        }
    }

    [Fact]
    public void IsCpf_WithEmptyString_ShouldReturnFalse()
    {
        // Arrange
        var cpf = "";

        // Act
        var result = CpfUtils.IsCpf(cpf);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsCpf_WithWhitespaceOnly_ShouldReturnFalse()
    {
        // Arrange
        var cpf = "   ";

        // Act
        var result = CpfUtils.IsCpf(cpf);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CleanCpf_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var cpf = "";

        // Act
        var result = CpfUtils.CleanCpf(cpf);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CleanCpf_WithWhitespaceOnly_ShouldReturnEmptyString()
    {
        // Arrange
        var cpf = "   ";

        // Act
        var result = CpfUtils.CleanCpf(cpf);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CleanCpf_WithOnlyDots_ShouldRemoveThem()
    {
        // Arrange
        var cpf = "...";

        // Act
        var result = CpfUtils.CleanCpf(cpf);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CleanCpf_WithOnlyDashes_ShouldRemoveThem()
    {
        // Arrange
        var cpf = "---";

        // Act
        var result = CpfUtils.CleanCpf(cpf);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void IsCpf_WithFormattedValidCpf_ShouldReturnTrue()
    {
        // Arrange
        var cpf = "529.982.247-25";

        // Act
        var result = CpfUtils.IsCpf(cpf);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsCpf_WithUnformattedValidCpf_ShouldReturnTrue()
    {
        // Arrange
        var cpf = "52998224725";

        // Act
        var result = CpfUtils.IsCpf(cpf);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsCpf_WithWrongCheckDigits_ShouldReturnFalse()
    {
        // Arrange - CPF com dígitos verificadores trocados
        var cpf = "52998224726";

        // Act
        var result = CpfUtils.IsCpf(cpf);

        // Assert
        result.Should().BeFalse();
    }
}
