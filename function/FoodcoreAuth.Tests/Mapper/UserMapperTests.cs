using FluentAssertions;
using Foodcore.Auth.DTO;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Mapper;

namespace FoodcoreAuth.Tests.Mapper;

public class UserMapperTests
{
    // ==========================================
    // Cenários VÁLIDOS
    // ==========================================

    [Fact]
    public void ToModel_WithEmailNameAndCpf_ShouldReturnCompleteUser()
    {
        // Arrange - Cenário: Email + Nome + CPF (válido)
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "Password1!",
            Cpf = "12345678909"
        };

        // Act
        var user = UserMapper.ToModel(dto);

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be("João Silva");
        user.Email?.Value.Should().Be("joao@example.com");
        user.Password?.Value.Should().Be("Password1!");
        user.Cpf?.Value.Should().Be("12345678909");
    }

    [Fact]
    public void ToModel_WithEmailAndName_ShouldReturnUserWithoutCpf()
    {
        // Arrange - Cenário: Email + Nome (válido, sem CPF)
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "", // Sem senha
            Cpf = ""       // Sem CPF
        };

        // Act
        var user = UserMapper.ToModel(dto);

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be("João Silva");
        user.Email?.Value.Should().Be("joao@example.com");
        user.Password.Should().BeNull();
        user.Cpf.Should().BeNull();
    }

    [Fact]
    public void ToModel_WithOnlyCpf_ShouldReturnUserWithGeneratedEmailAndName()
    {
        // Arrange - Cenário: Apenas CPF (válido, gera email/nome automático)
        var dto = new UserCreateDTO
        {
            Name = "",     // Vazio
            Email = "",    // Vazio
            Password = "", // Vazio
            Cpf = "12345678909"
        };

        // Act
        var user = UserMapper.ToModel(dto);

        // Assert
        user.Should().NotBeNull();
        user.Cpf?.Value.Should().Be("12345678909");
        // Quando só tem CPF, o User gera um GUID para email e nome
        user.Email.Should().NotBeNull();
        user.Email?.Value.Should().EndWith("@foodcore.com");
        user.Name.Should().NotBeNullOrEmpty();
        user.Password.Should().BeNull();
    }

    [Fact]
    public void ToModel_WithEmailNameAndCpfWithoutPassword_ShouldReturnUserWithoutPassword()
    {
        // Arrange - Cenário: Email + Nome + CPF sem senha (válido)
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "",
            Cpf = "12345678909"
        };

        // Act
        var user = UserMapper.ToModel(dto);

        // Assert
        user.Should().NotBeNull();
        user.Password.Should().BeNull();
        user.Cpf?.Value.Should().Be("12345678909");
    }

    // ==========================================
    // Cenários INVÁLIDOS
    // ==========================================

    [Fact]
    public void ToModel_WithNullDto_ShouldThrowBusinessException()
    {
        // Act
        Action act = () => UserMapper.ToModel(null!);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Dados do usuário inválidos.");
    }

    [Fact]
    public void ToModel_WithOnlyName_ShouldThrowBusinessException()
    {
        // Arrange - Cenário: Apenas Nome (INVÁLIDO - precisa de Email ou CPF)
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "",
            Password = "",
            Cpf = ""
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.");
    }

    [Fact]
    public void ToModel_WithOnlyEmail_ShouldThrowBusinessException()
    {
        // Arrange - Cenário: Apenas Email (INVÁLIDO - precisa de Nome também)
        var dto = new UserCreateDTO
        {
            Name = "",
            Email = "joao@example.com",
            Password = "",
            Cpf = ""
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.");
    }

    [Fact]
    public void ToModel_WithEmptyFields_ShouldThrowBusinessException()
    {
        // Arrange - Cenário: Tudo vazio (INVÁLIDO)
        var dto = new UserCreateDTO
        {
            Name = "",
            Email = "",
            Password = "",
            Cpf = ""
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.");
    }

    [Fact]
    public void ToModel_WithPasswordButWithoutCpf_ShouldThrowBusinessException()
    {
        // Arrange - Cenário: Email + Nome + Senha, mas SEM CPF (INVÁLIDO - senha só com todos os campos)
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "Password1!",
            Cpf = ""
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: ao informar uma senha, deve também informar Email, Nome e CPF.");
    }

    [Fact]
    public void ToModel_WithPasswordAndCpfOnly_ShouldThrowBusinessException()
    {
        // Arrange - Cenário: CPF + Senha, mas sem Email/Nome (INVÁLIDO)
        var dto = new UserCreateDTO
        {
            Name = "",
            Email = "",
            Password = "Password1!",
            Cpf = "12345678909"
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: ao informar uma senha, deve também informar Email, Nome e CPF.");
    }

    [Fact]
    public void ToModel_WithInvalidCpf_ShouldThrowBusinessException()
    {
        // Arrange - CPF inválido
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "",
            Cpf = "12345678900" // CPF inválido (dígitos verificadores errados)
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("O CPF informado é inválido.");
    }

    [Fact]
    public void ToModel_WithInvalidEmail_ShouldThrowBusinessException()
    {
        // Arrange - Email sem @
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "invalid-email",
            Password = "",
            Cpf = ""
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("O Email informado é inválido.");
    }

    [Fact]
    public void ToModel_WithInvalidPassword_ShouldThrowBusinessException()
    {
        // Arrange - Senha muito fraca
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "weak",
            Cpf = "12345678909"
        };

        // Act
        Action act = () => UserMapper.ToModel(dto);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    // ==========================================
    // Testes de comportamento do User
    // ==========================================

    [Fact]
    public void ToModel_UserWithPassword_ShouldNotBeCustomer()
    {
        // Arrange
        var dto = new UserCreateDTO
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "Password1!",
            Cpf = "12345678909"
        };

        // Act
        var user = UserMapper.ToModel(dto);

        // Assert
        user.IsCustomer().Should().BeFalse();
        user.GetRole().Should().Be(Foodcore.Auth.Model.Role.ADMIN);
    }

    [Fact]
    public void ToModel_UserWithoutPassword_ShouldBeCustomer()
    {
        // Arrange
        var dto = new UserCreateDTO
        {
            Name = "Customer User",
            Email = "customer@example.com",
            Password = "",
            Cpf = "12345678909"
        };

        // Act
        var user = UserMapper.ToModel(dto);

        // Assert
        user.IsCustomer().Should().BeTrue();
        user.GetRole().Should().Be(Foodcore.Auth.Model.Role.CUSTOMER);
    }

    [Fact]
    public void ToModel_ShouldGenerateUsernameFromEmail()
    {
        // Arrange
        var dto = new UserCreateDTO
        {
            Name = "João Silva",
            Email = "joao.silva@example.com",
            Password = "",
            Cpf = ""
        };

        // Act
        var user = UserMapper.ToModel(dto);

        // Assert
        user.Username.Should().Be("joao.silva");
    }
}
