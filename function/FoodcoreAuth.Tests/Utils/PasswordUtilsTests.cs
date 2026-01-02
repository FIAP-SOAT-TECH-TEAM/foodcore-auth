using FluentAssertions;
using Foodcore.Auth.Utils;

namespace FoodcoreAuth.Tests.Utils;

public class PasswordUtilsTests
{
    [Fact]
    public void GenerateTemporaryPassword_WithDefaultSettings_ShouldReturnValidPassword()
    {
        // Act
        var password = PasswordUtils.GenerateTemporaryPassword();

        // Assert
        password.Should().NotBeNullOrEmpty();
        password.Should().HaveLength(8);
        password.Should().ContainAny("abcdefghijklmnopqrstuvwxyz".ToCharArray().Select(c => c.ToString()).ToArray());
        password.Should().ContainAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(c => c.ToString()).ToArray());
        password.Should().ContainAny("0123456789".ToCharArray().Select(c => c.ToString()).ToArray());
    }

    [Theory]
    [InlineData(8)]
    [InlineData(12)]
    [InlineData(16)]
    [InlineData(20)]
    public void GenerateTemporaryPassword_WithCustomLength_ShouldReturnPasswordWithCorrectLength(int length)
    {
        // Act
        var password = PasswordUtils.GenerateTemporaryPassword(length);

        // Assert
        password.Should().HaveLength(length);
    }

    [Fact]
    public void GenerateTemporaryPassword_WithLowercaseOnly_ShouldContainOnlyLowercase()
    {
        // Act
        var password = PasswordUtils.GenerateTemporaryPassword(
            length: 10,
            requireLowercase: true,
            requireUppercase: false,
            requireNumbers: false,
            requireSymbols: false);

        // Assert
        password.Should().MatchRegex("^[a-z]+$");
    }

    [Fact]
    public void GenerateTemporaryPassword_WithUppercaseOnly_ShouldContainOnlyUppercase()
    {
        // Act
        var password = PasswordUtils.GenerateTemporaryPassword(
            length: 10,
            requireLowercase: false,
            requireUppercase: true,
            requireNumbers: false,
            requireSymbols: false);

        // Assert
        password.Should().MatchRegex("^[A-Z]+$");
    }

    [Fact]
    public void GenerateTemporaryPassword_WithNumbersOnly_ShouldContainOnlyNumbers()
    {
        // Act
        var password = PasswordUtils.GenerateTemporaryPassword(
            length: 10,
            requireLowercase: false,
            requireUppercase: false,
            requireNumbers: true,
            requireSymbols: false);

        // Assert
        password.Should().MatchRegex("^[0-9]+$");
    }

    [Fact]
    public void GenerateTemporaryPassword_WithNoCharacterSets_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => PasswordUtils.GenerateTemporaryPassword(
            length: 10,
            requireLowercase: false,
            requireUppercase: false,
            requireNumbers: false,
            requireSymbols: false);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("No character sets defined.");
    }

    [Fact]
    public void GenerateTemporaryPassword_ShouldGenerateDifferentPasswords()
    {
        // Act
        var password1 = PasswordUtils.GenerateTemporaryPassword();
        var password2 = PasswordUtils.GenerateTemporaryPassword();
        var password3 = PasswordUtils.GenerateTemporaryPassword();

        // Assert - pelo menos duas devem ser diferentes (estatisticamente muito improvável serem iguais)
        var passwords = new[] { password1, password2, password3 };
        passwords.Distinct().Count().Should().BeGreaterThan(1);
    }
}
