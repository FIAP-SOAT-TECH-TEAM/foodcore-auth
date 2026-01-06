using FluentAssertions;
using Foodcore.Auth.Model;

namespace FoodcoreAuth.Tests.Model;

public class SecurityRulesTests
{
    [Fact]
    public void SecurityRules_ShouldSetHttpMethod()
    {
        // Arrange & Act
        var rules = new SecurityRules
        {
            HttpMethod = "GET",
            Pattern = @"^/test$"
        };

        // Assert
        rules.HttpMethod.Should().Be("GET");
    }

    [Fact]
    public void SecurityRules_ShouldSetPattern()
    {
        // Arrange & Act
        var rules = new SecurityRules
        {
            HttpMethod = "POST",
            Pattern = @"^/order/\d+$"
        };

        // Assert
        rules.Pattern.Should().Be(@"^/order/\d+$");
    }

    [Fact]
    public void SecurityRules_WhenAllowedRolesIsNull_ShouldIndicatePublicEndpoint()
    {
        // Arrange & Act
        var rules = new SecurityRules
        {
            HttpMethod = "POST",
            Pattern = @"^/webhook$",
            AllowedRoles = null
        };

        // Assert
        rules.AllowedRoles.Should().BeNull();
    }

    [Fact]
    public void SecurityRules_ShouldSetAllowedRoles()
    {
        // Arrange & Act
        var rules = new SecurityRules
        {
            HttpMethod = "GET",
            Pattern = @"^/admin$",
            AllowedRoles = new HashSet<string> { "ADMIN", "CUSTOMER" }
        };

        // Assert
        rules.AllowedRoles.Should().NotBeNull();
        rules.AllowedRoles.Should().Contain("ADMIN");
        rules.AllowedRoles.Should().Contain("CUSTOMER");
        rules.AllowedRoles.Should().HaveCount(2);
    }

    [Fact]
    public void SecurityRules_AllowedRoles_ShouldBeHashSet()
    {
        // Arrange
        var roles = new HashSet<string> { "ADMIN", "ADMIN", "CUSTOMER" };

        // Act
        var rules = new SecurityRules
        {
            HttpMethod = "GET",
            Pattern = @"^/test$",
            AllowedRoles = roles
        };

        // Assert - HashSet não permite duplicatas
        rules.AllowedRoles.Should().HaveCount(2);
    }
}
