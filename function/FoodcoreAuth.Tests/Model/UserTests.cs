using FluentAssertions;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model;
using Foodcore.Auth.Model.ValueObjects;
using Role = Foodcore.Auth.Model.Role;

namespace FoodcoreAuth.Tests.Model;

public class UserTests
{
    #region Constructor - Valid Scenarios

    [Fact]
    public void Constructor_WithEmailAndName_ShouldCreateUser()
    {
        // Arrange
        var email = new Email("joao@example.com");

        // Act
        var user = new User("João Silva", email, null, null);

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be("João Silva");
        user.Email?.Value.Should().Be("joao@example.com");
        user.Username.Should().Be("joao");
        user.Cpf.Should().BeNull();
        user.Password.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithOnlyCpf_ShouldCreateUserWithGeneratedEmailAndName()
    {
        // Arrange
        var cpf = new Cpf("12345678909");

        // Act
        var user = new User("", null, null, cpf);

        // Assert
        user.Should().NotBeNull();
        user.Cpf?.Value.Should().Be("12345678909");
        user.Email.Should().NotBeNull();
        user.Email?.Value.Should().EndWith("@foodcore.com");
        user.Name.Should().NotBeNullOrEmpty();
        user.Username.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_WithEmailNameAndCpf_ShouldCreateCompleteUser()
    {
        // Arrange
        var email = new Email("joao@example.com");
        var cpf = new Cpf("12345678909");

        // Act
        var user = new User("João Silva", email, null, cpf);

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be("João Silva");
        user.Email?.Value.Should().Be("joao@example.com");
        user.Cpf?.Value.Should().Be("12345678909");
    }

    [Fact]
    public void Constructor_WithAllFields_ShouldCreateAdminUser()
    {
        // Arrange
        var email = new Email("admin@example.com");
        var password = new Password("AdminPass1!");
        var cpf = new Cpf("12345678909");

        // Act
        var user = new User("Admin User", email, password, cpf);

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be("Admin User");
        user.Email?.Value.Should().Be("admin@example.com");
        user.Password?.Value.Should().Be("AdminPass1!");
        user.Cpf?.Value.Should().Be("12345678909");
    }

    #endregion

    #region Constructor - Invalid Scenarios

    [Fact]
    public void Constructor_WithOnlyName_ShouldThrowBusinessException()
    {
        // Act
        Action act = () => new User("João Silva", null, null, null);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.");
    }

    [Fact]
    public void Constructor_WithOnlyEmail_ShouldThrowBusinessException()
    {
        // Arrange
        var email = new Email("joao@example.com");

        // Act
        Action act = () => new User("", email, null, null);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.");
    }

    [Fact]
    public void Constructor_WithPasswordButWithoutCpf_ShouldThrowBusinessException()
    {
        // Arrange
        var email = new Email("joao@example.com");
        var password = new Password("Password1!");

        // Act
        Action act = () => new User("João Silva", email, password, null);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: ao informar uma senha, deve também informar Email, Nome e CPF.");
    }

    [Fact]
    public void Constructor_WithPasswordAndCpfOnly_ShouldThrowBusinessException()
    {
        // Arrange
        var password = new Password("Password1!");
        var cpf = new Cpf("12345678909");

        // Act
        Action act = () => new User("", null, password, cpf);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("Usuário inválido: ao informar uma senha, deve também informar Email, Nome e CPF.");
    }

    #endregion

    #region Username Generation

    [Fact]
    public void Username_ShouldBeExtractedFromEmail()
    {
        // Arrange
        var email = new Email("joao.silva@example.com");

        // Act
        var user = new User("João Silva", email, null, null);

        // Assert
        user.Username.Should().Be("joao.silva");
    }

    [Fact]
    public void Username_WithSimpleEmail_ShouldExtractCorrectly()
    {
        // Arrange
        var email = new Email("admin@test.com");

        // Act
        var user = new User("Admin", email, null, null);

        // Assert
        user.Username.Should().Be("admin");
    }

    #endregion

    #region IsCustomer Tests

    [Fact]
    public void IsCustomer_WhenPasswordIsNull_ShouldReturnTrue()
    {
        // Arrange
        var email = new Email("customer@example.com");
        var user = new User("Customer", email, null, null);

        // Act
        var isCustomer = user.IsCustomer();

        // Assert
        isCustomer.Should().BeTrue();
    }

    [Fact]
    public void IsCustomer_WhenPasswordIsSet_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email("admin@example.com");
        var password = new Password("AdminPass1!");
        var cpf = new Cpf("12345678909");
        var user = new User("Admin", email, password, cpf);

        // Act
        var isCustomer = user.IsCustomer();

        // Assert
        isCustomer.Should().BeFalse();
    }

    [Fact]
    public void IsCustomer_WhenOnlyCpf_ShouldReturnTrue()
    {
        // Arrange
        var cpf = new Cpf("12345678909");
        var user = new User("", null, null, cpf);

        // Act
        var isCustomer = user.IsCustomer();

        // Assert
        isCustomer.Should().BeTrue();
    }

    #endregion

    #region GetRole Tests

    [Fact]
    public void GetRole_WhenIsCustomer_ShouldReturnCustomerRole()
    {
        // Arrange
        var email = new Email("customer@example.com");
        var user = new User("Customer", email, null, null);

        // Act
        var role = user.GetRole();

        // Assert
        role.Should().Be(Role.CUSTOMER);
    }

    [Fact]
    public void GetRole_WhenIsAdmin_ShouldReturnAdminRole()
    {
        // Arrange
        var email = new Email("admin@example.com");
        var password = new Password("AdminPass1!");
        var cpf = new Cpf("12345678909");
        var user = new User("Admin", email, password, cpf);

        // Act
        var role = user.GetRole();

        // Assert
        role.Should().Be(Role.ADMIN);
    }

    #endregion

    #region Property Setters

    [Fact]
    public void Properties_ShouldBeModifiable()
    {
        // Arrange
        var email = new Email("test@example.com");
        var user = new User("Original Name", email, null, null);

        // Act
        user.Name = "New Name";

        // Assert
        user.Name.Should().Be("New Name");
    }

    #endregion
}
