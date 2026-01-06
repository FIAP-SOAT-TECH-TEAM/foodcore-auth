using FluentAssertions;
using Foodcore.Auth.DTO;
using Foodcore.Auth.Presenter;

namespace FoodcoreAuth.Tests.Presenter;

public class CommonPresenterTests
{
    #region ToErrorDTO Tests

    [Fact]
    public void ToErrorDTO_WithValidException_ShouldReturnCorrectDTO()
    {
        // Arrange
        var exception = new Exception("Test error message");
        var statusCode = 400;
        var path = "/api/users";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(400);
        result.Message.Should().Be("Test error message");
        result.Path.Should().Be("/api/users");
    }

    [Fact]
    public void ToErrorDTO_WithNullException_ShouldThrowArgumentException()
    {
        // Arrange
        Exception exception = null!;

        // Act
        Action act = () => CommonPresenter.ToErrorDTO(exception, 500, "/api/test");

        // Assert
        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("ex");
    }

    [Fact]
    public void ToErrorDTO_WithNotFoundError_ShouldReturn404Status()
    {
        // Arrange
        var exception = new Exception("Resource not found");
        var statusCode = 404;
        var path = "/api/users/123";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Status.Should().Be(404);
        result.Message.Should().Be("Resource not found");
    }

    [Fact]
    public void ToErrorDTO_WithInternalServerError_ShouldReturn500Status()
    {
        // Arrange
        var exception = new InvalidOperationException("Internal server error");
        var statusCode = 500;
        var path = "/api/auth/login";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Status.Should().Be(500);
        result.Message.Should().Be("Internal server error");
    }

    [Fact]
    public void ToErrorDTO_WithUnauthorizedError_ShouldReturn401Status()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Access denied");
        var statusCode = 401;
        var path = "/api/protected";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Status.Should().Be(401);
        result.Message.Should().Be("Access denied");
    }

    [Fact]
    public void ToErrorDTO_WithForbiddenError_ShouldReturn403Status()
    {
        // Arrange
        var exception = new Exception("Forbidden");
        var statusCode = 403;
        var path = "/api/admin";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Status.Should().Be(403);
    }

    [Fact]
    public void ToErrorDTO_WithEmptyPath_ShouldReturnDTOWithEmptyPath()
    {
        // Arrange
        var exception = new Exception("Error");
        var statusCode = 400;
        var path = "";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Path.Should().BeEmpty();
    }

    [Fact]
    public void ToErrorDTO_ShouldReturnErrorDTOType()
    {
        // Arrange
        var exception = new Exception("Test");

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, 400, "/test");

        // Assert
        result.Should().BeOfType<ErrorDTO>();
    }

    [Fact]
    public void ToErrorDTO_WithNestedExceptionMessage_ShouldReturnTopLevelMessage()
    {
        // Arrange
        var innerException = new Exception("Inner error");
        var outerException = new Exception("Outer error", innerException);

        // Act
        var result = CommonPresenter.ToErrorDTO(outerException, 500, "/api/test");

        // Assert
        result.Message.Should().Be("Outer error");
    }

    [Fact]
    public void ToErrorDTO_WithArgumentException_ShouldReturnCorrectMessage()
    {
        // Arrange
        var exception = new ArgumentException("Invalid argument", "paramName");
        var statusCode = 400;
        var path = "/api/users";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Status.Should().Be(400);
        result.Message.Should().Contain("Invalid argument");
    }

    [Fact]
    public void ToErrorDTO_WithZeroStatusCode_ShouldReturnZeroStatus()
    {
        // Arrange
        var exception = new Exception("Test");
        var statusCode = 0;
        var path = "/test";

        // Act
        var result = CommonPresenter.ToErrorDTO(exception, statusCode, path);

        // Assert
        result.Status.Should().Be(0);
    }

    #endregion
}
