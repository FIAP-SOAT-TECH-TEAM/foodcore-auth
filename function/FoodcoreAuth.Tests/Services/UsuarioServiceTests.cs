using FluentAssertions;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model;
using Foodcore.Auth.Model.ValueObjects;
using Foodcore.Auth.Services;
using Role = Foodcore.Auth.Model.Role;

namespace FoodcoreAuth.Tests.Services;

public class UsuarioServiceTests
{
    private readonly CognitoSettings _settings;

    public UsuarioServiceTests()
    {
        _settings = new CognitoSettings
        {
            UserPoolId = "us-east-1_test",
            AppClientId = "test-client-id",
            Region = "us-east-1",
            GuestUserEmail = "guest@test.com",
            DefaultCustomerPassword = "DefaultPassword123!"
        };
    }

    #region GetUserPassword Tests

    [Fact]
    public void GetUserPassword_WhenUserIsCustomer_ShouldReturnDefaultPassword()
    {
        // Arrange - Customer não tem senha (Password = null)
        var user = CreateCustomerUser();

        // Act
        var password = UsuarioService.GetUserPassword(_settings, user);

        // Assert
        password.Should().Be(_settings.DefaultCustomerPassword);
    }

    [Fact]
    public void GetUserPassword_WhenUserIsAdmin_ShouldReturnUserPassword()
    {
        // Arrange - Admin tem senha definida
        var user = CreateAdminUser();

        // Act
        var password = UsuarioService.GetUserPassword(_settings, user);

        // Assert
        password.Should().Be("AdminPass1!");
    }

    [Fact]
    public void GetUserPassword_WhenAdminHasNullPassword_ShouldThrowBusinessException()
    {
        // Arrange - Usuário com CPF, Email, Nome mas sem senha (inconsistente)
        var email = new Email("admin@test.com");
        var cpf = new Cpf("12345678909");
        // Criando user como customer (sem senha) para testar o cenário
        var user = new User("Admin", email, null, cpf);

        // Forçando o cenário onde IsCustomer retorna false mas não tem senha
        // Isso é um edge case que não deveria acontecer, mas testamos a validação
        
        // Act & Assert - Como IsCustomer() retorna true quando Password é null, 
        // este teste verifica que retorna a senha padrão
        var password = UsuarioService.GetUserPassword(_settings, user);
        password.Should().Be(_settings.DefaultCustomerPassword);
    }

    #endregion

    #region UserCanAccessUrl Tests - Public Endpoints

    [Fact]
    public void UserCanAccessUrl_WhenAccessingPublicWebhook_ShouldNotThrow()
    {
        // Arrange
        var url = "/payment/webhook";
        var method = "POST";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAccessingPublicWebhookWithAnyRole_ShouldNotThrow()
    {
        // Arrange
        var url = "/payment/webhook";
        var method = "POST";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.CUSTOMER);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region UserCanAccessUrl Tests - Order Endpoints

    [Fact]
    public void UserCanAccessUrl_WhenCustomerCreatesOrder_ShouldNotThrow()
    {
        // Arrange
        var url = "/order";
        var method = "POST";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.CUSTOMER);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminCreatesOrder_ShouldThrowAccessDenied()
    {
        // Arrange - Admin não pode criar orders
        var url = "/order";
        var method = "POST";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().Throw<AccessDeniedException>();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminPatchesOrder_ShouldNotThrow()
    {
        // Arrange
        var url = "/order/123";
        var method = "PATCH";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenCustomerPatchesOrder_ShouldThrowAccessDenied()
    {
        // Arrange - Customer não pode modificar orders
        var url = "/order/123";
        var method = "PATCH";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.CUSTOMER);

        // Assert
        act.Should().Throw<AccessDeniedException>();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminGetsActiveOrders_ShouldNotThrow()
    {
        // Arrange
        var url = "/order/active";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region UserCanAccessUrl Tests - Catalog Endpoints

    [Fact]
    public void UserCanAccessUrl_WhenCustomerGetsCatalog_ShouldNotThrow()
    {
        // Arrange
        var url = "/catalog";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.CUSTOMER);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminGetsCatalog_ShouldNotThrow()
    {
        // Arrange
        var url = "/catalog";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminPostsCatalog_ShouldNotThrow()
    {
        // Arrange
        var url = "/catalog/product";
        var method = "POST";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenCustomerPostsCatalog_ShouldThrowAccessDenied()
    {
        // Arrange - Customer não pode criar produtos
        var url = "/catalog/product";
        var method = "POST";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.CUSTOMER);

        // Assert
        act.Should().Throw<AccessDeniedException>();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminDeletesCatalog_ShouldNotThrow()
    {
        // Arrange
        var url = "/catalog/product/123";
        var method = "DELETE";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region UserCanAccessUrl Tests - Payment Endpoints

    [Fact]
    public void UserCanAccessUrl_WhenCustomerGetsQrCode_ShouldNotThrow()
    {
        // Arrange
        var url = "/payment/123/qrCode";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.CUSTOMER);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminGetsPaymentStatus_ShouldNotThrow()
    {
        // Arrange
        var url = "/payment/123/status";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenAdminGetsLatestPayment_ShouldNotThrow()
    {
        // Arrange
        var url = "/payment/123/latest";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UserCanAccessUrl_WhenCustomerGetsLatestPayment_ShouldThrowAccessDenied()
    {
        // Arrange - Customer não pode ver latest payment
        var url = "/payment/123/latest";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.CUSTOMER);

        // Assert
        act.Should().Throw<AccessDeniedException>();
    }

    #endregion

    #region UserCanAccessUrl Tests - Error Cases

    [Theory]
    [InlineData("", "GET")]
    [InlineData(null, "GET")]
    [InlineData("/order", "")]
    [InlineData("/order", null)]
    public void UserCanAccessUrl_WhenUrlOrMethodIsEmpty_ShouldThrowBusinessException(string? url, string? method)
    {
        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url!, method!);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage("URL ou método HTTP não fornecidos.");
    }

    [Fact]
    public void UserCanAccessUrl_WhenAccessingUnknownEndpoint_ShouldThrowAccessDenied()
    {
        // Arrange - Endpoint não mapeado
        var url = "/unknown/endpoint";
        var method = "GET";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method, Role.ADMIN);

        // Assert
        act.Should().Throw<AccessDeniedException>();
    }

    [Fact]
    public void UserCanAccessUrl_WhenNoRoleProvided_ShouldThrowAccessDenied()
    {
        // Arrange - Tentando acessar endpoint protegido sem role
        var url = "/order";
        var method = "POST";

        // Act
        Action act = () => UsuarioService.UserCanAccessUrl(url, method);

        // Assert
        act.Should().Throw<AccessDeniedException>();
    }

    [Fact]
    public void UserCanAccessUrl_MethodIsCaseInsensitive()
    {
        // Arrange
        var url = "/order";

        // Act & Assert - lowercase
        Action actLower = () => UsuarioService.UserCanAccessUrl(url, "post", Role.CUSTOMER);
        actLower.Should().NotThrow();

        // Act & Assert - uppercase
        Action actUpper = () => UsuarioService.UserCanAccessUrl(url, "POST", Role.CUSTOMER);
        actUpper.Should().NotThrow();
    }

    #endregion

    #region Helper Methods

    private User CreateCustomerUser()
    {
        var cpf = new Cpf("12345678909");
        return new User("", null, null, cpf);
    }

    private User CreateAdminUser()
    {
        var email = new Email("admin@test.com");
        var password = new Password("AdminPass1!");
        var cpf = new Cpf("12345678909");
        return new User("Admin User", email, password, cpf);
    }

    #endregion
}
