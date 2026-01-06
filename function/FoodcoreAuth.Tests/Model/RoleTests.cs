using FluentAssertions;
using Foodcore.Auth.Model;

namespace FoodcoreAuth.Tests.Model;

public class RoleTests
{
    #region Enum Values Tests

    [Fact]
    public void Role_ShouldHaveCustomerValue()
    {
        // Assert
        Role.CUSTOMER.Should().BeDefined();
    }

    [Fact]
    public void Role_ShouldHaveAdminValue()
    {
        // Assert
        Role.ADMIN.Should().BeDefined();
    }

    [Fact]
    public void Role_Customer_ToStringShouldReturnCorrectValue()
    {
        // Act
        var result = Role.CUSTOMER.ToString();

        // Assert
        result.Should().Be("CUSTOMER");
    }

    [Fact]
    public void Role_Admin_ToStringShouldReturnCorrectValue()
    {
        // Act
        var result = Role.ADMIN.ToString();

        // Assert
        result.Should().Be("ADMIN");
    }

    [Fact]
    public void Role_ShouldHaveOnlyTwoValues()
    {
        // Act
        var values = Enum.GetValues<Role>();

        // Assert
        values.Should().HaveCount(2);
    }

    [Fact]
    public void Role_ShouldBeEnumType()
    {
        // Assert
        typeof(Role).IsEnum.Should().BeTrue();
    }

    [Fact]
    public void Role_CanBeParsedFromString()
    {
        // Act
        var result = Enum.Parse<Role>("CUSTOMER");

        // Assert
        result.Should().Be(Role.CUSTOMER);
    }

    [Fact]
    public void Role_CanBeParsedFromStringCaseInsensitive()
    {
        // Act
        var result = Enum.Parse<Role>("admin", ignoreCase: true);

        // Assert
        result.Should().Be(Role.ADMIN);
    }

    [Fact]
    public void Role_TryParseWithInvalidValue_ShouldReturnFalse()
    {
        // Act
        var success = Enum.TryParse<Role>("INVALID", out var result);

        // Assert
        success.Should().BeFalse();
    }

    [Fact]
    public void Role_TryParseWithValidValue_ShouldReturnTrue()
    {
        // Act
        var success = Enum.TryParse<Role>("ADMIN", out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(Role.ADMIN);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void Role_Customer_ShouldNotEqualAdmin()
    {
        // Assert
        Role.CUSTOMER.Should().NotBe(Role.ADMIN);
    }

    [Fact]
    public void Role_Admin_ShouldEqualAdmin()
    {
        // Arrange
        var role1 = Role.ADMIN;
        var role2 = Role.ADMIN;

        // Assert
        role1.Should().Be(role2);
    }

    #endregion
}
