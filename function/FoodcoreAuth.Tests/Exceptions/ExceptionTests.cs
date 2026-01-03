using FluentAssertions;
using Foodcore.Auth.Exceptions;

namespace FoodcoreAuth.Tests.Exceptions;

public class AccessDeniedExceptionTests
{
    [Fact]
    public void AccessDeniedException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new AccessDeniedException("Test message");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void AccessDeniedException_ShouldSetMessageCorrectly()
    {
        // Arrange
        var message = "Access denied to resource";

        // Act
        var exception = new AccessDeniedException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void AccessDeniedException_WithEmptyMessage_ShouldHaveEmptyMessage()
    {
        // Arrange & Act
        var exception = new AccessDeniedException("");

        // Assert
        exception.Message.Should().BeEmpty();
    }

    [Fact]
    public void AccessDeniedException_CanBeThrown()
    {
        // Arrange
        var message = "Test exception";

        // Act
        Action act = () => throw new AccessDeniedException(message);

        // Assert
        act.Should().Throw<AccessDeniedException>()
            .WithMessage(message);
    }

    [Fact]
    public void AccessDeniedException_CanBeCaught()
    {
        // Arrange
        var message = "Caught exception";
        AccessDeniedException? caught = null;

        // Act
        try
        {
            throw new AccessDeniedException(message);
        }
        catch (AccessDeniedException ex)
        {
            caught = ex;
        }

        // Assert
        caught.Should().NotBeNull();
        caught!.Message.Should().Be(message);
    }
}

public class BusinessExceptionTests
{
    [Fact]
    public void BusinessException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new BusinessException("Test message");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void BusinessException_ShouldSetMessageCorrectly()
    {
        // Arrange
        var message = "Business rule violation";

        // Act
        var exception = new BusinessException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void BusinessException_WithEmptyMessage_ShouldHaveEmptyMessage()
    {
        // Arrange & Act
        var exception = new BusinessException("");

        // Assert
        exception.Message.Should().BeEmpty();
    }

    [Fact]
    public void BusinessException_CanBeThrown()
    {
        // Arrange
        var message = "Invalid CPF";

        // Act
        Action act = () => throw new BusinessException(message);

        // Assert
        act.Should().Throw<BusinessException>()
            .WithMessage(message);
    }

    [Fact]
    public void BusinessException_CanBeCaught()
    {
        // Arrange
        var message = "Caught business exception";
        BusinessException? caught = null;

        // Act
        try
        {
            throw new BusinessException(message);
        }
        catch (BusinessException ex)
        {
            caught = ex;
        }

        // Assert
        caught.Should().NotBeNull();
        caught!.Message.Should().Be(message);
    }

    [Fact]
    public void BusinessException_UsedForUserValidation_ShouldHaveDescriptiveMessage()
    {
        // Arrange
        var message = "Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.";

        // Act
        var exception = new BusinessException(message);

        // Assert
        exception.Message.Should().Contain("Usuário inválido");
    }

    [Fact]
    public void BusinessException_UsedForPasswordValidation_ShouldHaveDescriptiveMessage()
    {
        // Arrange
        var message = "Usuário inválido: ao informar uma senha, deve também informar Email, Nome e CPF.";

        // Act
        var exception = new BusinessException(message);

        // Assert
        exception.Message.Should().Contain("senha");
    }
}
