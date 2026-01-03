using FluentAssertions;
using Foodcore.Auth.Config;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Newtonsoft.Json.Serialization;

namespace FoodcoreAuth.Tests.Config;

public class OpenApiConfigurationOptionsTests
{
    private readonly OpenApiConfigurationOptions _config = new();

    [Fact]
    public void Info_ShouldHaveCorrectVersion()
    {
        // Assert
        _config.Info.Version.Should().Be("1.0.0");
    }

    [Fact]
    public void Info_ShouldHaveCorrectTitle()
    {
        // Assert
        _config.Info.Title.Should().Be("Foodcore Auth API");
    }

    [Fact]
    public void Info_ShouldHaveCorrectDescription()
    {
        // Assert
        _config.Info.Description.Should().Contain("AWS Cognito");
    }

    [Fact]
    public void Info_ShouldHaveContactName()
    {
        // Assert
        _config.Info.Contact.Should().NotBeNull();
        _config.Info.Contact.Name.Should().Be("SOAT Team 8");
    }

    [Fact]
    public void Info_ShouldHaveContactUrl()
    {
        // Assert
        _config.Info.Contact.Url.Should().NotBeNull();
        _config.Info.Contact.Url!.ToString().Should().Contain("github.com");
    }

    [Fact]
    public void Info_ShouldHaveLicense()
    {
        // Assert
        _config.Info.License.Should().NotBeNull();
        _config.Info.License.Name.Should().Be("MIT");
    }

    [Fact]
    public void Info_ShouldHaveLicenseUrl()
    {
        // Assert
        _config.Info.License.Url.Should().NotBeNull();
        _config.Info.License.Url!.ToString().Should().Contain("MIT");
    }

    [Fact]
    public void OpenApiVersion_ShouldBeV3()
    {
        // Assert
        _config.OpenApiVersion.Should().Be(OpenApiVersionType.V3);
    }
}

public class AuthResponseDTOExampleTests
{
    [Fact]
    public void Build_ShouldReturnNonNullExample()
    {
        // Arrange
        var example = new AuthResponseDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        var result = example.Build(namingStrategy);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Build_ShouldContainExamples()
    {
        // Arrange
        var example = new AuthResponseDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        example.Build(namingStrategy);

        // Assert
        example.Examples.Should().NotBeEmpty();
    }
}

public class CustomerAuthDTOExampleTests
{
    [Fact]
    public void Build_ShouldReturnNonNullExample()
    {
        // Arrange
        var example = new CustomerAuthDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        var result = example.Build(namingStrategy);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Build_ShouldContainExamples()
    {
        // Arrange
        var example = new CustomerAuthDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        example.Build(namingStrategy);

        // Assert
        example.Examples.Should().NotBeEmpty();
    }
}

public class ErrorDTOExampleTests
{
    [Fact]
    public void Build_ShouldReturnNonNullExample()
    {
        // Arrange
        var example = new ErrorDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        var result = example.Build(namingStrategy);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Build_ShouldContainExamples()
    {
        // Arrange
        var example = new ErrorDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        example.Build(namingStrategy);

        // Assert
        example.Examples.Should().NotBeEmpty();
    }
}

public class UserCreatedResponseDTOExampleTests
{
    [Fact]
    public void Build_ShouldReturnNonNullExample()
    {
        // Arrange
        var example = new UserCreatedResponseDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        var result = example.Build(namingStrategy);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Build_ShouldContainExamples()
    {
        // Arrange
        var example = new UserCreatedResponseDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        example.Build(namingStrategy);

        // Assert
        example.Examples.Should().NotBeEmpty();
    }
}

public class UserCreateDTOExampleTests
{
    [Fact]
    public void Build_ShouldReturnNonNullExample()
    {
        // Arrange
        var example = new UserCreateDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        var result = example.Build(namingStrategy);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Build_ShouldContainExamples()
    {
        // Arrange
        var example = new UserCreateDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        example.Build(namingStrategy);

        // Assert
        example.Examples.Should().NotBeEmpty();
    }
}

public class UserDetailsDTOExampleTests
{
    [Fact]
    public void Build_ShouldReturnNonNullExample()
    {
        // Arrange
        var example = new UserDetailsDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        var result = example.Build(namingStrategy);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Build_ShouldContainExamples()
    {
        // Arrange
        var example = new UserDetailsDTOExample();
        var namingStrategy = new CamelCaseNamingStrategy();

        // Act
        example.Build(namingStrategy);

        // Assert
        example.Examples.Should().NotBeEmpty();
    }
}
