using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using FluentAssertions;
using Foodcore.Auth.Model;
using Foodcore.Auth.Model.ValueObjects;
using Foodcore.Auth.Services;
using Moq;

namespace FoodcoreAuth.Tests.Services;

public class CognitoServiceTests
{
    private readonly Mock<IAmazonCognitoIdentityProvider> _cognitoMock;
    private readonly CognitoSettings _settings;

    public CognitoServiceTests()
    {
        _cognitoMock = new Mock<IAmazonCognitoIdentityProvider>();
        _settings = new CognitoSettings
        {
            UserPoolId = "us-east-1_test",
            AppClientId = "test-client-id",
            Region = "us-east-1",
            GuestUserEmail = "guest@test.com",
            DefaultCustomerPassword = "DefaultPassword123!"
        };
    }

    #region GetUserByEmailOrCpfAsync Tests

    [Fact]
    public async Task GetUserByEmailOrCpfAsync_WhenUserFoundByEmail_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new UserType
        {
            Username = "testuser",
            Attributes = new List<AttributeType>
            {
                new() { Name = "email", Value = "test@test.com" }
            }
        };

        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")),
            default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType> { expectedUser }
            });

        // Act
        var result = await CognitoService.GetUserByEmailOrCpfAsync(
            _cognitoMock.Object,
            _settings,
            "test@test.com",
            "");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetUserByEmailOrCpfAsync_WhenUserFoundByCpf_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new UserType
        {
            Username = "cpfuser",
            Attributes = new List<AttributeType>
            {
                new() { Name = "preferred_username", Value = "12345678909" }
            }
        };

        // First call (by email) returns empty
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email")),
            default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Second call (by cpf) returns user
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("preferred_username")),
            default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType> { expectedUser }
            });

        // Act
        var result = await CognitoService.GetUserByEmailOrCpfAsync(
            _cognitoMock.Object,
            _settings,
            "",
            "123.456.789-09");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("cpfuser");
    }

    [Fact]
    public async Task GetUserByEmailOrCpfAsync_WhenNoUserFound_ShouldReturnNull()
    {
        // Arrange
        _cognitoMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Act
        var result = await CognitoService.GetUserByEmailOrCpfAsync(
            _cognitoMock.Object,
            _settings,
            "notfound@test.com",
            "");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetUserBySubAsync Tests

    [Fact]
    public async Task GetUserBySubAsync_WhenUserFound_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new UserType
        {
            Username = "subuser",
            Attributes = new List<AttributeType>
            {
                new() { Name = "sub", Value = "abc-123" }
            }
        };

        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("sub")),
            default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType> { expectedUser }
            });

        // Act
        var result = await CognitoService.GetUserBySubAsync(
            _cognitoMock.Object,
            _settings,
            "abc-123");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("subuser");
    }

    [Fact]
    public async Task GetUserBySubAsync_WhenNoUserFound_ShouldReturnNull()
    {
        // Arrange
        _cognitoMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Act
        var result = await CognitoService.GetUserBySubAsync(
            _cognitoMock.Object,
            _settings,
            "nonexistent-sub");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_WithValidUser_ShouldCallCognitoWithCorrectParameters()
    {
        // Arrange
        var email = new Email("test@test.com");
        var cpf = new Cpf("12345678909");
        var password = new Password("Password1!");
        var user = new User("Test User", email, password, cpf);

        var expectedResponse = new AdminCreateUserResponse
        {
            User = new UserType
            {
                Username = "test",
                Attributes = new List<AttributeType>
                {
                    new() { Name = "sub", Value = "new-user-sub" }
                }
            }
        };

        _cognitoMock.Setup(c => c.AdminCreateUserAsync(
            It.IsAny<AdminCreateUserRequest>(),
            default))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await CognitoService.CreateUser(
            _cognitoMock.Object,
            _settings,
            user);

        // Assert
        result.Should().NotBeNull();
        result.User.Username.Should().Be("test");

        _cognitoMock.Verify(c => c.AdminCreateUserAsync(
            It.Is<AdminCreateUserRequest>(r =>
                r.UserPoolId == _settings.UserPoolId &&
                r.Username == user.Username &&
                r.MessageAction == "SUPPRESS"),
            default), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ShouldIncludeCustomRoleAttribute()
    {
        // Arrange
        var email = new Email("admin@test.com");
        var cpf = new Cpf("12345678909");
        var password = new Password("AdminPass1!");
        var user = new User("Admin User", email, password, cpf);

        AdminCreateUserRequest? capturedRequest = null;
        _cognitoMock.Setup(c => c.AdminCreateUserAsync(
            It.IsAny<AdminCreateUserRequest>(),
            default))
            .Callback<AdminCreateUserRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new AdminCreateUserResponse
            {
                User = new UserType { Username = "admin" }
            });

        // Act
        await CognitoService.CreateUser(_cognitoMock.Object, _settings, user);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.UserAttributes.Should().Contain(a => 
            a.Name == "custom:role" && a.Value == "ADMIN");
    }

    #endregion

    #region UpdateUserPassword Tests

    [Fact]
    public async Task UpdateUserPassword_ShouldCallCognitoWithCorrectParameters()
    {
        // Arrange
        var username = "testuser";
        var newPassword = "NewPassword1!";

        _cognitoMock.Setup(c => c.AdminSetUserPasswordAsync(
            It.IsAny<AdminSetUserPasswordRequest>(),
            default))
            .ReturnsAsync(new AdminSetUserPasswordResponse());

        // Act
        await CognitoService.UpdateUserPassword(
            _cognitoMock.Object,
            _settings,
            username,
            newPassword);

        // Assert
        _cognitoMock.Verify(c => c.AdminSetUserPasswordAsync(
            It.Is<AdminSetUserPasswordRequest>(r =>
                r.UserPoolId == _settings.UserPoolId &&
                r.Username == username &&
                r.Password == newPassword &&
                r.Permanent == true),
            default), Times.Once);
    }

    #endregion
}
