using System.Security.Claims;
using Amazon.CognitoIdentityProvider.Model;
using FluentAssertions;
using Foodcore.Auth.DTO;
using Foodcore.Auth.Presenter;

namespace FoodcoreAuth.Tests.Presenter;

public class UserPresenterTests
{
    #region ToUserCreatedResponse Tests

    [Fact]
    public void ToUserCreatedResponse_WithValidResponse_ShouldReturnCorrectDTO()
    {
        // Arrange
        var cognitoResponse = new AdminCreateUserResponse
        {
            User = new UserType
            {
                Attributes = new List<AttributeType>
                {
                    new AttributeType { Name = "sub", Value = "abc-123-def-456" },
                    new AttributeType { Name = "email", Value = "test@example.com" }
                }
            }
        };

        // Act
        var result = UserPresenter.ToUserCreatedResponse(cognitoResponse);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be("Usuário criado com sucesso!");
        result.UserSub.Should().Be("abc-123-def-456");
    }

    [Fact]
    public void ToUserCreatedResponse_WithNullResponse_ShouldThrowArgumentException()
    {
        // Arrange
        AdminCreateUserResponse response = null!;

        // Act
        Action act = () => UserPresenter.ToUserCreatedResponse(response);

        // Assert
        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("user");
    }

    [Fact]
    public void ToUserCreatedResponse_ShouldReturnUserCreatedResponseDTOType()
    {
        // Arrange
        var cognitoResponse = new AdminCreateUserResponse
        {
            User = new UserType
            {
                Attributes = new List<AttributeType>
                {
                    new AttributeType { Name = "sub", Value = "test-sub" }
                }
            }
        };

        // Act
        var result = UserPresenter.ToUserCreatedResponse(cognitoResponse);

        // Assert
        result.Should().BeOfType<UserCreatedResponseDTO>();
    }

    #endregion

    #region ToUserDetailsDTO Tests

    [Fact]
    public void ToUserDetailsDTO_WithUserAndClaims_ShouldReturnCompleteDTO()
    {
        // Arrange
        var user = new UserType
        {
            Attributes = new List<AttributeType>
            {
                new AttributeType { Name = "name", Value = "João Silva" },
                new AttributeType { Name = "email", Value = "joao@example.com" },
                new AttributeType { Name = "custom:cpf", Value = "12345678909" },
                new AttributeType { Name = "custom:role", Value = "ADMIN" }
            },
            UserCreateDate = new DateTime(2024, 1, 15, 10, 30, 0)
        };

        var claims = new List<Claim>
        {
            new Claim("sub", "user-sub-123")
        };

        // Act
        var result = UserPresenter.ToUserDetailsDTO(user, claims);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().Be("user-sub-123");
        result.Name.Should().Be("João Silva");
        result.Email.Should().Be("joao@example.com");
        result.Cpf.Should().Be("12345678909");
        result.Role.Should().Be("ADMIN");
        result.CreatedAt.Should().Be(new DateTime(2024, 1, 15, 10, 30, 0));
    }

    [Fact]
    public void ToUserDetailsDTO_WithNullUser_ShouldReturnDTOWithDefaults()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("sub", "user-sub-456")
        };

        // Act
        var result = UserPresenter.ToUserDetailsDTO(null, claims);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().Be("user-sub-456");
        result.Name.Should().BeEmpty();
        result.Email.Should().BeEmpty();
        result.Cpf.Should().BeEmpty();
        result.Role.Should().BeEmpty();
        result.CreatedAt.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void ToUserDetailsDTO_WithNullClaims_ShouldReturnDTOWithEmptySubject()
    {
        // Arrange
        var user = new UserType
        {
            Attributes = new List<AttributeType>
            {
                new AttributeType { Name = "name", Value = "Maria" },
                new AttributeType { Name = "email", Value = "maria@test.com" }
            },
            UserCreateDate = new DateTime(2024, 2, 20)
        };

        // Act
        var result = UserPresenter.ToUserDetailsDTO(user, null);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().BeEmpty();
        result.Name.Should().Be("Maria");
        result.Email.Should().Be("maria@test.com");
    }

    [Fact]
    public void ToUserDetailsDTO_WithBothNull_ShouldReturnDTOWithAllDefaults()
    {
        // Act
        var result = UserPresenter.ToUserDetailsDTO(null, null);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().BeEmpty();
        result.Name.Should().BeEmpty();
        result.Email.Should().BeEmpty();
        result.Cpf.Should().BeEmpty();
        result.Role.Should().BeEmpty();
        result.CreatedAt.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void ToUserDetailsDTO_WithEmptyClaims_ShouldReturnDTOWithEmptySubject()
    {
        // Arrange
        var claims = new List<Claim>();

        // Act
        var result = UserPresenter.ToUserDetailsDTO(null, claims);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().BeEmpty();
    }

    [Fact]
    public void ToUserDetailsDTO_WithPartialUserAttributes_ShouldReturnDTOWithAvailableData()
    {
        // Arrange
        var user = new UserType
        {
            Attributes = new List<AttributeType>
            {
                new AttributeType { Name = "name", Value = "Test User" }
            }
        };

        // Act
        var result = UserPresenter.ToUserDetailsDTO(user, null);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test User");
        result.Email.Should().BeEmpty();
        result.Cpf.Should().BeEmpty();
        result.Role.Should().BeEmpty();
    }

    [Fact]
    public void ToUserDetailsDTO_ShouldReturnUserDetailsDTOType()
    {
        // Act
        var result = UserPresenter.ToUserDetailsDTO(null, null);

        // Assert
        result.Should().BeOfType<UserDetailsDTO>();
    }

    [Fact]
    public void ToUserDetailsDTO_WithCustomerRole_ShouldReturnCustomerRole()
    {
        // Arrange
        var user = new UserType
        {
            Attributes = new List<AttributeType>
            {
                new AttributeType { Name = "custom:role", Value = "CUSTOMER" }
            }
        };

        // Act
        var result = UserPresenter.ToUserDetailsDTO(user, null);

        // Assert
        result.Role.Should().Be("CUSTOMER");
    }

    #endregion
}
