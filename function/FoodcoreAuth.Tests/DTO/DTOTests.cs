using FluentAssertions;
using Foodcore.Auth.DTO;

namespace FoodcoreAuth.Tests.DTO;

public class AuthResponseDTOTests
{
    [Fact]
    public void AuthResponseDTO_ShouldSetIdTokenCorrectly()
    {
        // Arrange & Act
        var dto = new AuthResponseDTO
        {
            IdToken = "id-token-123",
            AccessToken = "access",
            RefreshToken = "refresh",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Assert
        dto.IdToken.Should().Be("id-token-123");
    }

    [Fact]
    public void AuthResponseDTO_ShouldSetAccessTokenCorrectly()
    {
        // Arrange & Act
        var dto = new AuthResponseDTO
        {
            IdToken = "id",
            AccessToken = "access-token-456",
            RefreshToken = "refresh",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Assert
        dto.AccessToken.Should().Be("access-token-456");
    }

    [Fact]
    public void AuthResponseDTO_ShouldSetRefreshTokenCorrectly()
    {
        // Arrange & Act
        var dto = new AuthResponseDTO
        {
            IdToken = "id",
            AccessToken = "access",
            RefreshToken = "refresh-token-789",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Assert
        dto.RefreshToken.Should().Be("refresh-token-789");
    }

    [Fact]
    public void AuthResponseDTO_ShouldSetExpiresInCorrectly()
    {
        // Arrange & Act
        var dto = new AuthResponseDTO
        {
            IdToken = "id",
            AccessToken = "access",
            RefreshToken = "refresh",
            ExpiresIn = 7200,
            TokenType = "Bearer"
        };

        // Assert
        dto.ExpiresIn.Should().Be(7200);
    }

    [Fact]
    public void AuthResponseDTO_ShouldSetTokenTypeCorrectly()
    {
        // Arrange & Act
        var dto = new AuthResponseDTO
        {
            IdToken = "id",
            AccessToken = "access",
            RefreshToken = "refresh",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Assert
        dto.TokenType.Should().Be("Bearer");
    }
}

public class CustomerAuthDTOTests
{
    [Fact]
    public void CustomerAuthDTO_ShouldHaveDefaultEmptyEmail()
    {
        // Arrange & Act
        var dto = new CustomerAuthDTO();

        // Assert
        dto.Email.Should().BeEmpty();
    }

    [Fact]
    public void CustomerAuthDTO_ShouldHaveDefaultEmptyCpf()
    {
        // Arrange & Act
        var dto = new CustomerAuthDTO();

        // Assert
        dto.Cpf.Should().BeEmpty();
    }

    [Fact]
    public void CustomerAuthDTO_ShouldSetEmailCorrectly()
    {
        // Arrange & Act
        var dto = new CustomerAuthDTO
        {
            Email = "test@example.com"
        };

        // Assert
        dto.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void CustomerAuthDTO_ShouldSetCpfCorrectly()
    {
        // Arrange & Act
        var dto = new CustomerAuthDTO
        {
            Cpf = "12345678909"
        };

        // Assert
        dto.Cpf.Should().Be("12345678909");
    }
}

public class ErrorDTOTests
{
    [Fact]
    public void ErrorDTO_ShouldSetTimestampAutomatically()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var dto = new ErrorDTO
        {
            Status = 400,
            Message = "Error",
            Path = "/api/test"
        };

        var after = DateTime.UtcNow;

        // Assert
        dto.Timestamp.Should().BeOnOrAfter(before.AddSeconds(-1));
        dto.Timestamp.Should().BeOnOrBefore(after.AddSeconds(1));
    }

    [Fact]
    public void ErrorDTO_ShouldSetStatusCorrectly()
    {
        // Arrange & Act
        var dto = new ErrorDTO
        {
            Status = 404,
            Message = "Not found",
            Path = "/api/users/123"
        };

        // Assert
        dto.Status.Should().Be(404);
    }

    [Fact]
    public void ErrorDTO_ShouldSetMessageCorrectly()
    {
        // Arrange & Act
        var dto = new ErrorDTO
        {
            Status = 500,
            Message = "Internal server error",
            Path = "/api/test"
        };

        // Assert
        dto.Message.Should().Be("Internal server error");
    }

    [Fact]
    public void ErrorDTO_ShouldSetPathCorrectly()
    {
        // Arrange & Act
        var dto = new ErrorDTO
        {
            Status = 400,
            Message = "Error",
            Path = "/api/users/create"
        };

        // Assert
        dto.Path.Should().Be("/api/users/create");
    }
}

public class UserCreatedResponseDTOTests
{
    [Fact]
    public void UserCreatedResponseDTO_ShouldSetMessageCorrectly()
    {
        // Arrange & Act
        var dto = new UserCreatedResponseDTO
        {
            Message = "User created successfully",
            UserSub = "sub-123"
        };

        // Assert
        dto.Message.Should().Be("User created successfully");
    }

    [Fact]
    public void UserCreatedResponseDTO_ShouldSetUserSubCorrectly()
    {
        // Arrange & Act
        var dto = new UserCreatedResponseDTO
        {
            Message = "Success",
            UserSub = "abc-def-123-456"
        };

        // Assert
        dto.UserSub.Should().Be("abc-def-123-456");
    }
}

public class UserCreateDTOTests
{
    [Fact]
    public void UserCreateDTO_ShouldHaveDefaultEmptyName()
    {
        // Arrange & Act
        var dto = new UserCreateDTO();

        // Assert
        dto.Name.Should().BeEmpty();
    }

    [Fact]
    public void UserCreateDTO_ShouldHaveDefaultEmptyEmail()
    {
        // Arrange & Act
        var dto = new UserCreateDTO();

        // Assert
        dto.Email.Should().BeEmpty();
    }

    [Fact]
    public void UserCreateDTO_ShouldHaveDefaultEmptyPassword()
    {
        // Arrange & Act
        var dto = new UserCreateDTO();

        // Assert
        dto.Password.Should().BeEmpty();
    }

    [Fact]
    public void UserCreateDTO_ShouldHaveDefaultEmptyCpf()
    {
        // Arrange & Act
        var dto = new UserCreateDTO();

        // Assert
        dto.Cpf.Should().BeEmpty();
    }

    [Fact]
    public void UserCreateDTO_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "Senha123!",
            Cpf = "12345678909"
        };

        // Assert
        dto.Name.Should().Be("João Silva");
        dto.Email.Should().Be("joao@example.com");
        dto.Password.Should().Be("Senha123!");
        dto.Cpf.Should().Be("12345678909");
    }
}

public class UserDetailsDTOTests
{
    [Fact]
    public void UserDetailsDTO_ShouldSetSubjectCorrectly()
    {
        // Arrange & Act
        var dto = new UserDetailsDTO
        {
            Subject = "sub-123",
            Name = "Test",
            Email = "test@test.com",
            Cpf = "12345678909",
            Role = "ADMIN",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Subject.Should().Be("sub-123");
    }

    [Fact]
    public void UserDetailsDTO_ShouldSetNameCorrectly()
    {
        // Arrange & Act
        var dto = new UserDetailsDTO
        {
            Subject = "sub-123",
            Name = "João Silva",
            Email = "test@test.com",
            Cpf = "12345678909",
            Role = "ADMIN",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Name.Should().Be("João Silva");
    }

    [Fact]
    public void UserDetailsDTO_ShouldSetEmailCorrectly()
    {
        // Arrange & Act
        var dto = new UserDetailsDTO
        {
            Subject = "sub-123",
            Name = "Test",
            Email = "joao@example.com",
            Cpf = "12345678909",
            Role = "ADMIN",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Email.Should().Be("joao@example.com");
    }

    [Fact]
    public void UserDetailsDTO_ShouldSetCpfCorrectly()
    {
        // Arrange & Act
        var dto = new UserDetailsDTO
        {
            Subject = "sub-123",
            Name = "Test",
            Email = "test@test.com",
            Cpf = "12345678909",
            Role = "ADMIN",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Cpf.Should().Be("12345678909");
    }

    [Fact]
    public void UserDetailsDTO_ShouldSetRoleCorrectly()
    {
        // Arrange & Act
        var dto = new UserDetailsDTO
        {
            Subject = "sub-123",
            Name = "Test",
            Email = "test@test.com",
            Cpf = "12345678909",
            Role = "CUSTOMER",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Role.Should().Be("CUSTOMER");
    }

    [Fact]
    public void UserDetailsDTO_ShouldSetCreatedAtCorrectly()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var dto = new UserDetailsDTO
        {
            Subject = "sub-123",
            Name = "Test",
            Email = "test@test.com",
            Cpf = "12345678909",
            Role = "ADMIN",
            CreatedAt = createdAt
        };

        // Assert
        dto.CreatedAt.Should().Be(createdAt);
    }
}
