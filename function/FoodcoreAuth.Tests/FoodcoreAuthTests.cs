using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using FluentAssertions;
using Foodcore.Auth;
using Foodcore.Auth.DTO;
using Foodcore.Auth.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FoodcoreAuthFunction = Foodcore.Auth.FoodcoreAuth;

namespace FoodcoreAuth.Tests;

public class FoodcoreAuthTests
{
    private readonly Mock<ILogger<FoodcoreAuthFunction>> _loggerMock;
    private readonly Mock<IAmazonCognitoIdentityProvider> _cognitoMock;
    private readonly CognitoSettings _settings;
    private readonly FoodcoreAuthFunction _function;

    public FoodcoreAuthTests()
    {
        _loggerMock = new Mock<ILogger<FoodcoreAuthFunction>>();
        _cognitoMock = new Mock<IAmazonCognitoIdentityProvider>();
        _settings = new CognitoSettings
        {
            UserPoolId = "us-east-1_test",
            AppClientId = "test-client-id",
            Region = "us-east-1",
            GuestUserEmail = "guest@test.com",
            DefaultCustomerPassword = "DefaultPassword123!"
        };
        _function = new FoodcoreAuthFunction(_loggerMock.Object, _cognitoMock.Object, _settings);
    }

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var userCreateDTO = new UserCreateDTO
        {
            Name = "Test User",
            Email = "test@test.com",
            Cpf = "529.982.247-25",
            Password = "Password1!"
        };

        var httpRequest = CreateMockHttpRequest(userCreateDTO);

        // Mock - no existing user
        _cognitoMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Mock - create user
        _cognitoMock.Setup(c => c.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), default))
            .ReturnsAsync(new AdminCreateUserResponse
            {
                User = new UserType
                {
                    Username = "testuser",
                    Attributes = new List<AttributeType>
                    {
                        new() { Name = "sub", Value = "test-sub-123" }
                    }
                }
            });

        // Mock - set password
        _cognitoMock.Setup(c => c.AdminSetUserPasswordAsync(It.IsAny<AdminSetUserPasswordRequest>(), default))
            .ReturnsAsync(new AdminSetUserPasswordResponse());

        // Act
        var result = await _function.CreateUser(httpRequest, userCreateDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<UserCreatedResponseDTO>();
    }

    [Fact]
    public async Task CreateUser_WithExistingUser_ShouldReturnBadRequest()
    {
        // Arrange
        var userCreateDTO = new UserCreateDTO
        {
            Name = "Test User",
            Email = "existing@test.com",
            Cpf = "529.982.247-25",
            Password = "Password1!"
        };

        var httpRequest = CreateMockHttpRequest(userCreateDTO);

        // Mock - user already exists
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")), default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType>
                {
                    new UserType { Username = "existinguser" }
                }
            });

        // Act
        var result = await _function.CreateUser(httpRequest, userCreateDTO);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateUser_WithInvalidCpf_ShouldReturnBadRequest()
    {
        // Arrange
        var userCreateDTO = new UserCreateDTO
        {
            Name = "Test User",
            Email = "test@test.com",
            Cpf = "123.456.789-00", // Invalid CPF
            Password = "Password1!"
        };

        var httpRequest = CreateMockHttpRequest(userCreateDTO);

        // Act
        var result = await _function.CreateUser(httpRequest, userCreateDTO);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateUser_WithMissingName_ShouldReturnBadRequest()
    {
        // Arrange
        var userCreateDTO = new UserCreateDTO
        {
            Name = "", // Missing name
            Email = "test@test.com",
            Cpf = "529.982.247-25",
            Password = "Password1!"
        };

        var httpRequest = CreateMockHttpRequest(userCreateDTO);

        // Act
        var result = await _function.CreateUser(httpRequest, userCreateDTO);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateUser_WhenCognitoThrowsException_ShouldReturnInternalServerError()
    {
        // Arrange
        var userCreateDTO = new UserCreateDTO
        {
            Name = "Test User",
            Email = "test@test.com",
            Cpf = "529.982.247-25",
            Password = "Password1!"
        };

        var httpRequest = CreateMockHttpRequest(userCreateDTO);

        // Mock - no existing user
        _cognitoMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Mock - create user throws exception
        _cognitoMock.Setup(c => c.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), default))
            .ThrowsAsync(new Exception("Cognito error"));

        // Act
        var result = await _function.CreateUser(httpRequest, userCreateDTO);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region AuthCustomer Tests

    [Fact]
    public async Task AuthCustomer_WithValidCredentials_ShouldReturnOkResult()
    {
        // Arrange
        var customerAuthDTO = new CustomerAuthDTO
        {
            Email = "customer@test.com",
            Cpf = "529.982.247-25"
        };

        var httpRequest = CreateMockHttpRequest(customerAuthDTO);

        // Mock - user found
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")), default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType>
                {
                    new UserType
                    {
                        Username = "customeruser",
                        Attributes = new List<AttributeType>
                        {
                            new() { Name = "email", Value = "customer@test.com" },
                            new() { Name = "custom:role", Value = "CUSTOMER" }
                        }
                    }
                }
            });

        // Mock - authentication success
        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(It.IsAny<AdminInitiateAuthRequest>(), default))
            .ReturnsAsync(new AdminInitiateAuthResponse
            {
                AuthenticationResult = new AuthenticationResultType
                {
                    AccessToken = "test-access-token",
                    IdToken = "test-id-token",
                    RefreshToken = "test-refresh-token",
                    ExpiresIn = 3600,
                    TokenType = "Bearer"
                }
            });

        // Act
        var result = await _function.AuthCustomer(httpRequest, customerAuthDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<AuthResponseDTO>();
    }

    [Fact]
    public async Task AuthCustomer_WithGuestAuthentication_ShouldReturnOkResult()
    {
        // Arrange
        var customerAuthDTO = new CustomerAuthDTO
        {
            Email = null,
            Cpf = null
        };

        var httpRequest = CreateMockHttpRequest(customerAuthDTO);

        // Mock - guest user found
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")), default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType>
                {
                    new UserType
                    {
                        Username = "guestuser",
                        Attributes = new List<AttributeType>
                        {
                            new() { Name = "email", Value = "guest@test.com" },
                            new() { Name = "custom:role", Value = "CUSTOMER" }
                        }
                    }
                }
            });

        // Mock - authentication success
        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(It.IsAny<AdminInitiateAuthRequest>(), default))
            .ReturnsAsync(new AdminInitiateAuthResponse
            {
                AuthenticationResult = new AuthenticationResultType
                {
                    AccessToken = "guest-access-token",
                    IdToken = "guest-id-token",
                    RefreshToken = "guest-refresh-token",
                    ExpiresIn = 3600,
                    TokenType = "Bearer"
                }
            });

        // Act
        var result = await _function.AuthCustomer(httpRequest, customerAuthDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AuthCustomer_WhenUserNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var customerAuthDTO = new CustomerAuthDTO
        {
            Email = "notfound@test.com",
            Cpf = ""
        };

        var httpRequest = CreateMockHttpRequest(customerAuthDTO);

        // Mock - no user found
        _cognitoMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Act
        var result = await _function.AuthCustomer(httpRequest, customerAuthDTO);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AuthCustomer_WhenUserIsNotCustomer_ShouldReturnBadRequest()
    {
        // Arrange
        var customerAuthDTO = new CustomerAuthDTO
        {
            Email = "admin@test.com",
            Cpf = ""
        };

        var httpRequest = CreateMockHttpRequest(customerAuthDTO);

        // Mock - admin user found (not customer)
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")), default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType>
                {
                    new UserType
                    {
                        Username = "adminuser",
                        Attributes = new List<AttributeType>
                        {
                            new() { Name = "email", Value = "admin@test.com" },
                            new() { Name = "custom:role", Value = "ADMIN" }
                        }
                    }
                }
            });

        // Act
        var result = await _function.AuthCustomer(httpRequest, customerAuthDTO);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AuthCustomer_WhenNotAuthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var customerAuthDTO = new CustomerAuthDTO
        {
            Email = "customer@test.com",
            Cpf = ""
        };

        var httpRequest = CreateMockHttpRequest(customerAuthDTO);

        // Mock - customer found
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")), default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType>
                {
                    new UserType
                    {
                        Username = "customeruser",
                        Attributes = new List<AttributeType>
                        {
                            new() { Name = "email", Value = "customer@test.com" },
                            new() { Name = "custom:role", Value = "CUSTOMER" }
                        }
                    }
                }
            });

        // Mock - authentication fails
        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(It.IsAny<AdminInitiateAuthRequest>(), default))
            .ThrowsAsync(new NotAuthorizedException("Invalid credentials"));

        // Act
        var result = await _function.AuthCustomer(httpRequest, customerAuthDTO);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task AuthCustomer_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        var customerAuthDTO = new CustomerAuthDTO
        {
            Email = "customer@test.com",
            Cpf = ""
        };

        var httpRequest = CreateMockHttpRequest(customerAuthDTO);

        // Mock - customer found
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")), default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType>
                {
                    new UserType
                    {
                        Username = "customeruser",
                        Attributes = new List<AttributeType>
                        {
                            new() { Name = "email", Value = "customer@test.com" },
                            new() { Name = "custom:role", Value = "CUSTOMER" }
                        }
                    }
                }
            });

        // Mock - unexpected exception
        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(It.IsAny<AdminInitiateAuthRequest>(), default))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _function.AuthCustomer(httpRequest, customerAuthDTO);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task AuthCustomer_WhenGuestUserNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var customerAuthDTO = new CustomerAuthDTO
        {
            Email = null,
            Cpf = null
        };

        var httpRequest = CreateMockHttpRequest(customerAuthDTO);

        // Mock - guest user not found
        _cognitoMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Act
        var result = await _function.AuthCustomer(httpRequest, customerAuthDTO);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Helper Methods

    private static HttpRequestData CreateMockHttpRequest<T>(T body)
    {
        var context = new Mock<FunctionContext>();
        var request = new Mock<HttpRequestData>(context.Object);

        var json = JsonSerializer.Serialize(body);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        request.Setup(r => r.Body).Returns(stream);
        request.Setup(r => r.Url).Returns(new Uri("http://localhost/api/test"));
        request.Setup(r => r.Query).Returns(new MockQueryCollection());

        return request.Object;
    }

    private class MockQueryCollection : System.Collections.Specialized.NameValueCollection
    {
    }

    #endregion
}
