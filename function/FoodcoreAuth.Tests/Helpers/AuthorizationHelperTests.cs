using FluentAssertions;
using Foodcore.Auth.Helpers;
using Foodcore.Auth.Model;

namespace FoodcoreAuth.Tests.Helpers;

public class AuthorizationHelperTests
{
    #region Rules Collection Tests

    [Fact]
    public void Rules_ShouldNotBeNull()
    {
        // Act
        var rules = AuthorizationHelper.Rules;

        // Assert
        rules.Should().NotBeNull();
    }

    [Fact]
    public void Rules_ShouldNotBeEmpty()
    {
        // Act
        var rules = AuthorizationHelper.Rules;

        // Assert
        rules.Should().NotBeEmpty();
    }

    [Fact]
    public void Rules_ShouldContainSecurityRulesType()
    {
        // Act
        var rules = AuthorizationHelper.Rules;

        // Assert
        rules.Should().AllBeOfType<SecurityRules>();
    }

    #endregion

    #region Public Endpoints Tests

    [Fact]
    public void Rules_PaymentWebhook_ShouldHaveNullAllowedRoles()
    {
        // Act
        var webhookRule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/payment/webhook$");

        // Assert
        webhookRule.Should().NotBeNull();
        webhookRule!.HttpMethod.Should().Be("POST");
        webhookRule.AllowedRoles.Should().BeNull();
    }

    #endregion

    #region Order Endpoints Tests

    [Fact]
    public void Rules_PostOrder_ShouldAllowCustomerOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/order$" && r.HttpMethod == "POST");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.CUSTOMER.ToString());
        rule.AllowedRoles.Should().NotContain(Role.ADMIN.ToString());
    }

    [Fact]
    public void Rules_PatchOrder_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/order/.*$" && r.HttpMethod == "PATCH");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
        rule.AllowedRoles.Should().NotContain(Role.CUSTOMER.ToString());
    }

    [Fact]
    public void Rules_GetOrderActive_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/order/active$");

        // Assert
        rule.Should().NotBeNull();
        rule!.HttpMethod.Should().Be("GET");
        rule.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
    }

    #endregion

    #region Catalog Endpoints Tests

    [Fact]
    public void Rules_GetCatalog_ShouldAllowAdminAndCustomer()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/catalog.*$" && r.HttpMethod == "GET");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
        rule.AllowedRoles.Should().Contain(Role.CUSTOMER.ToString());
    }

    [Fact]
    public void Rules_PostCatalog_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/catalog.*$" && r.HttpMethod == "POST");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
        rule.AllowedRoles.Should().NotContain(Role.CUSTOMER.ToString());
    }

    [Fact]
    public void Rules_PatchCatalog_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/catalog.*$" && r.HttpMethod == "PATCH");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
        rule.AllowedRoles.Should().HaveCount(1);
    }

    [Fact]
    public void Rules_PutCatalog_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/catalog.*$" && r.HttpMethod == "PUT");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
    }

    [Fact]
    public void Rules_DeleteCatalog_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/catalog.*$" && r.HttpMethod == "DELETE");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
    }

    #endregion

    #region Payment Endpoints Tests

    [Fact]
    public void Rules_GetPaymentQrCode_ShouldAllowAdminAndCustomer()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/payment/[^/]+/qrCode$");

        // Assert
        rule.Should().NotBeNull();
        rule!.HttpMethod.Should().Be("GET");
        rule.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
        rule.AllowedRoles.Should().Contain(Role.CUSTOMER.ToString());
    }

    [Fact]
    public void Rules_GetPaymentStatus_ShouldAllowAdminAndCustomer()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/payment/[^/]+/status$");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().HaveCount(2);
    }

    [Fact]
    public void Rules_GetPaymentLatest_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/payment/[^/]+/latest$");

        // Assert
        rule.Should().NotBeNull();
        rule!.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
        rule.AllowedRoles.Should().HaveCount(1);
    }

    [Fact]
    public void Rules_GetMerchantOrders_ShouldAllowAdminOnly()
    {
        // Act
        var rule = AuthorizationHelper.Rules
            .FirstOrDefault(r => r.Pattern == @"^/payment/merchant_orders/[^/]+$");

        // Assert
        rule.Should().NotBeNull();
        rule!.HttpMethod.Should().Be("GET");
        rule.AllowedRoles.Should().Contain(Role.ADMIN.ToString());
    }

    #endregion

    #region Rules Count and Validity Tests

    [Fact]
    public void Rules_AllRules_ShouldHaveValidHttpMethod()
    {
        // Arrange
        var validMethods = new[] { "GET", "POST", "PUT", "PATCH", "DELETE" };

        // Act
        var rules = AuthorizationHelper.Rules;

        // Assert
        rules.Should().AllSatisfy(r => 
            validMethods.Should().Contain(r.HttpMethod));
    }

    [Fact]
    public void Rules_AllRules_ShouldHaveNonEmptyPattern()
    {
        // Act
        var rules = AuthorizationHelper.Rules;

        // Assert
        rules.Should().AllSatisfy(r => 
            r.Pattern.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void Rules_ShouldContainCorrectNumberOfRules()
    {
        // Act
        var rules = AuthorizationHelper.Rules;

        // Assert
        rules.Should().HaveCountGreaterOrEqualTo(10);
    }

    #endregion
}
