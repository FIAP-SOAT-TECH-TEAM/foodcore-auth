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

    #region AuthenticateUserAsync Tests

    [Fact]
    public async Task AuthenticateUserAsync_WithValidCredentials_ShouldReturnAuthResult()
    {
        // Arrange
        var username = "testuser";
        var password = "Password123!";

        var expectedAuthResult = new AuthenticationResultType
        {
            AccessToken = "test-access-token",
            IdToken = "test-id-token",
            RefreshToken = "test-refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(
            It.IsAny<AdminInitiateAuthRequest>(),
            default))
            .ReturnsAsync(new AdminInitiateAuthResponse
            {
                AuthenticationResult = expectedAuthResult
            });

        // Act
        var result = await CognitoService.AuthenticateUserAsync(
            _cognitoMock.Object,
            _settings,
            username,
            password);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("test-access-token");
        result.IdToken.Should().Be("test-id-token");
        result.RefreshToken.Should().Be("test-refresh-token");
        result.ExpiresIn.Should().Be(3600);
        result.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public async Task AuthenticateUserAsync_ShouldCallCognitoWithCorrectParameters()
    {
        // Arrange
        var username = "testuser";
        var password = "Password123!";

        AdminInitiateAuthRequest? capturedRequest = null;
        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(
            It.IsAny<AdminInitiateAuthRequest>(),
            default))
            .Callback<AdminInitiateAuthRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new AdminInitiateAuthResponse
            {
                AuthenticationResult = new AuthenticationResultType
                {
                    AccessToken = "token"
                }
            });

        // Act
        await CognitoService.AuthenticateUserAsync(
            _cognitoMock.Object,
            _settings,
            username,
            password);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.UserPoolId.Should().Be(_settings.UserPoolId);
        capturedRequest.ClientId.Should().Be(_settings.AppClientId);
        capturedRequest.AuthFlow.Should().Be(AuthFlowType.ADMIN_USER_PASSWORD_AUTH);
        capturedRequest.AuthParameters.Should().ContainKey("USERNAME");
        capturedRequest.AuthParameters.Should().ContainKey("PASSWORD");
        capturedRequest.AuthParameters["USERNAME"].Should().Be(username);
        capturedRequest.AuthParameters["PASSWORD"].Should().Be(password);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WhenNotAuthorized_ShouldThrow()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";

        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(
            It.IsAny<AdminInitiateAuthRequest>(),
            default))
            .ThrowsAsync(new NotAuthorizedException("Invalid credentials"));

        // Act & Assert
        await Assert.ThrowsAsync<NotAuthorizedException>(() =>
            CognitoService.AuthenticateUserAsync(
                _cognitoMock.Object,
                _settings,
                username,
                password));
    }

    [Fact]
    public async Task AuthenticateUserAsync_WhenUserNotFound_ShouldThrow()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "Password123!";

        _cognitoMock.Setup(c => c.AdminInitiateAuthAsync(
            It.IsAny<AdminInitiateAuthRequest>(),
            default))
            .ThrowsAsync(new UserNotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            CognitoService.AuthenticateUserAsync(
                _cognitoMock.Object,
                _settings,
                username,
                password));
    }

    #endregion

    #region GetUserByEmailOrCpfAsync Edge Cases

    [Fact]
    public async Task GetUserByEmailOrCpfAsync_WhenCpfHasFormatting_ShouldCleanAndSearch()
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

        // Second call (by cpf) returns user - verify CPF is cleaned
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("preferred_username") && r.Filter.Contains("12345678909")),
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
            "123.456.789-09"); // Formatted CPF

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("cpfuser");
    }

    [Fact]
    public async Task GetUserByEmailOrCpfAsync_WhenBothEmpty_ShouldSearchWithNoneValues()
    {
        // Arrange
        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("email = \"none\"")),
            default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        _cognitoMock.Setup(c => c.ListUsersAsync(
            It.Is<ListUsersRequest>(r => r.Filter.Contains("preferred_username = \"none\"")),
            default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        // Act
        var result = await CognitoService.GetUserByEmailOrCpfAsync(
            _cognitoMock.Object,
            _settings,
            "",
            "");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateUser Edge Cases

    [Fact]
    public async Task CreateUser_WithCustomerRole_ShouldSetCorrectRoleAttribute()
    {
        // Arrange - Customer com apenas CPF (sem email e nome)
        var cpf = new Cpf("12345678909");
        var user = new User("", null, null, cpf);

        AdminCreateUserRequest? capturedRequest = null;
        _cognitoMock.Setup(c => c.AdminCreateUserAsync(
            It.IsAny<AdminCreateUserRequest>(),
            default))
            .Callback<AdminCreateUserRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new AdminCreateUserResponse
            {
                User = new UserType { Username = "customer" }
            });

        // Act
        await CognitoService.CreateUser(_cognitoMock.Object, _settings, user);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.UserAttributes.Should().Contain(a => 
            a.Name == "custom:role" && a.Value == "CUSTOMER");
    }

    [Fact]
    public async Task CreateUser_ShouldSuppressEmailNotification()
    {
        // Arrange
        var email = new Email("test@test.com");
        var cpf = new Cpf("12345678909");
        var password = new Password("Password1!");
        var user = new User("Test User", email, password, cpf);

        AdminCreateUserRequest? capturedRequest = null;
        _cognitoMock.Setup(c => c.AdminCreateUserAsync(
            It.IsAny<AdminCreateUserRequest>(),
            default))
            .Callback<AdminCreateUserRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new AdminCreateUserResponse
            {
                User = new UserType { Username = "test" }
            });

        // Act
        await CognitoService.CreateUser(_cognitoMock.Object, _settings, user);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.MessageAction.Should().Be("SUPPRESS");
    }

    [Fact]
    public async Task CreateUser_ShouldSetAllRequiredAttributes()
    {
        // Arrange
        var email = new Email("test@example.com");
        var cpf = new Cpf("52998224725");
        var password = new Password("Password1!");
        var user = new User("Test User Name", email, password, cpf);

        AdminCreateUserRequest? capturedRequest = null;
        _cognitoMock.Setup(c => c.AdminCreateUserAsync(
            It.IsAny<AdminCreateUserRequest>(),
            default))
            .Callback<AdminCreateUserRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new AdminCreateUserResponse
            {
                User = new UserType { Username = "test" }
            });

        // Act
        await CognitoService.CreateUser(_cognitoMock.Object, _settings, user);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.UserAttributes.Should().Contain(a => a.Name == "name" && a.Value == "Test User Name");
        capturedRequest.UserAttributes.Should().Contain(a => a.Name == "email" && a.Value == "test@example.com");
        capturedRequest.UserAttributes.Should().Contain(a => a.Name == "preferred_username" && a.Value == "52998224725");
        capturedRequest.UserAttributes.Should().Contain(a => a.Name == "custom:cpf" && a.Value == "52998224725");
        capturedRequest.UserAttributes.Should().Contain(a => a.Name == "custom:role");
    }

    #endregion
}
