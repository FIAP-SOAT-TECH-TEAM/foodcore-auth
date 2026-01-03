using FluentAssertions;
using Foodcore.Auth.Model;

namespace FoodcoreAuth.Tests.Model;

public class CognitoSettingsTests
{
    [Fact]
    public void CognitoSettings_ShouldSetUserPoolId()
    {
        // Arrange & Act
        var settings = CreateSettings();

        // Assert
        settings.UserPoolId.Should().Be("us-east-1_testpool");
    }

    [Fact]
    public void CognitoSettings_ShouldSetAppClientId()
    {
        // Arrange & Act
        var settings = CreateSettings();

        // Assert
        settings.AppClientId.Should().Be("test-client-123");
    }

    [Fact]
    public void CognitoSettings_ShouldSetRegion()
    {
        // Arrange & Act
        var settings = CreateSettings();

        // Assert
        settings.Region.Should().Be("us-east-1");
    }

    [Fact]
    public void CognitoSettings_ShouldSetGuestUserEmail()
    {
        // Arrange & Act
        var settings = CreateSettings();

        // Assert
        settings.GuestUserEmail.Should().Be("guest@foodcore.com");
    }

    [Fact]
    public void CognitoSettings_ShouldSetDefaultCustomerPassword()
    {
        // Arrange & Act
        var settings = CreateSettings();

        // Assert
        settings.DefaultCustomerPassword.Should().Be("CustomerPass123!");
    }

    [Fact]
    public void CognitoSettings_AllPropertiesShouldBeSettable()
    {
        // Arrange
        var settings = new CognitoSettings
        {
            UserPoolId = "pool1",
            AppClientId = "client1",
            Region = "eu-west-1",
            GuestUserEmail = "guest@test.com",
            DefaultCustomerPassword = "Pass123!"
        };

        // Act - Modify properties
        settings.UserPoolId = "pool2";
        settings.AppClientId = "client2";
        settings.Region = "ap-southeast-1";
        settings.GuestUserEmail = "newguest@test.com";
        settings.DefaultCustomerPassword = "NewPass456!";

        // Assert
        settings.UserPoolId.Should().Be("pool2");
        settings.AppClientId.Should().Be("client2");
        settings.Region.Should().Be("ap-southeast-1");
        settings.GuestUserEmail.Should().Be("newguest@test.com");
        settings.DefaultCustomerPassword.Should().Be("NewPass456!");
    }

    private CognitoSettings CreateSettings()
    {
        return new CognitoSettings
        {
            UserPoolId = "us-east-1_testpool",
            AppClientId = "test-client-123",
            Region = "us-east-1",
            GuestUserEmail = "guest@foodcore.com",
            DefaultCustomerPassword = "CustomerPass123!"
        };
    }
}
