using Amazon.CognitoIdentityProvider.Model;
using FluentAssertions;
using Foodcore.Auth.DTO;
using Foodcore.Auth.Presenter;

namespace FoodcoreAuth.Tests.Presenter;

public class AuthPresenterTests
{
    #region ToAuthResponseDTO Tests

    [Fact]
    public void ToAuthResponseDTO_WithValidAuthResult_ShouldReturnCorrectDTO()
    {
        // Arrange
        var authResult = new AuthenticationResultType
        {
            IdToken = "id-token-123",
            AccessToken = "access-token-456",
            RefreshToken = "refresh-token-789",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Act
        var result = AuthPresenter.ToAuthResponseDTO(authResult);

        // Assert
        result.Should().NotBeNull();
        result.IdToken.Should().Be("id-token-123");
        result.AccessToken.Should().Be("access-token-456");
        result.RefreshToken.Should().Be("refresh-token-789");
        result.ExpiresIn.Should().Be(3600);
        result.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public void ToAuthResponseDTO_WithNullAuthResult_ShouldThrowArgumentException()
    {
        // Arrange
        AuthenticationResultType authResult = null!;

        // Act
        Action act = () => AuthPresenter.ToAuthResponseDTO(authResult);

        // Assert
        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("authenticationResultType");
    }

    [Fact]
    public void ToAuthResponseDTO_WithMinimalAuthResult_ShouldReturnDTOWithDefaults()
    {
        // Arrange
        var authResult = new AuthenticationResultType
        {
            IdToken = "id",
            AccessToken = "access",
            RefreshToken = "refresh",
            ExpiresIn = 0,
            TokenType = "Bearer"
        };

        // Act
        var result = AuthPresenter.ToAuthResponseDTO(authResult);

        // Assert
        result.Should().NotBeNull();
        result.ExpiresIn.Should().Be(0);
    }

    [Fact]
    public void ToAuthResponseDTO_ShouldReturnAuthResponseDTOType()
    {
        // Arrange
        var authResult = new AuthenticationResultType
        {
            IdToken = "id",
            AccessToken = "access",
            RefreshToken = "refresh",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Act
        var result = AuthPresenter.ToAuthResponseDTO(authResult);

        // Assert
        result.Should().BeOfType<AuthResponseDTO>();
    }

    #endregion
}
