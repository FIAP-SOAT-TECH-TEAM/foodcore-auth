using FluentAssertions;
using Foodcore.Auth.Utils;

namespace FoodcoreAuth.Tests.Utils;

public class AwsCredentialsUtilsTests
{
    #region GetAwsCredentialsDict - Valid Scenarios

    [Fact]
    public void GetAwsCredentialsDict_WithValidCredentials_ShouldReturnCorrectDict()
    {
        // Arrange
        var rawCreds = @"[default]
aws_access_key_id=AKIAIOSFODNN7EXAMPLE
aws_secret_access_key=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY
aws_session_token=FwoGZXIvYXdzEC...";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainKey("aws_access_key_id");
        result.Should().ContainKey("aws_secret_access_key");
        result.Should().ContainKey("aws_session_token");
        result["aws_access_key_id"].Should().Be("AKIAIOSFODNN7EXAMPLE");
        result["aws_secret_access_key"].Should().Be("wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithoutSectionHeader_ShouldReturnCorrectDict()
    {
        // Arrange
        var rawCreds = @"aws_access_key_id=ACCESS_KEY
aws_secret_access_key=SECRET_KEY
aws_session_token=SESSION_TOKEN";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result.Should().HaveCount(3);
        result["aws_access_key_id"].Should().Be("ACCESS_KEY");
        result["aws_secret_access_key"].Should().Be("SECRET_KEY");
        result["aws_session_token"].Should().Be("SESSION_TOKEN");
    }

    [Fact]
    public void GetAwsCredentialsDict_ShouldBeCaseInsensitive()
    {
        // Arrange
        var rawCreds = @"AWS_ACCESS_KEY_ID=TEST_ACCESS
AWS_SECRET_ACCESS_KEY=TEST_SECRET
AWS_SESSION_TOKEN=TEST_TOKEN";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result["aws_access_key_id"].Should().Be("TEST_ACCESS");
        result["AWS_ACCESS_KEY_ID"].Should().Be("TEST_ACCESS");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithExtraWhitespace_ShouldTrimValues()
    {
        // Arrange
        var rawCreds = @"  aws_access_key_id  =  ACCESS_KEY  
  aws_secret_access_key  =  SECRET_KEY  
  aws_session_token  =  SESSION_TOKEN  ";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result["aws_access_key_id"].Should().Be("ACCESS_KEY");
        result["aws_secret_access_key"].Should().Be("SECRET_KEY");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithBlankLines_ShouldIgnoreThem()
    {
        // Arrange
        var rawCreds = @"
aws_access_key_id=ACCESS_KEY

aws_secret_access_key=SECRET_KEY

aws_session_token=SESSION_TOKEN
";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public void GetAwsCredentialsDict_WithValueContainingEquals_ShouldSplitOnFirstEquals()
    {
        // Arrange
        var rawCreds = @"aws_access_key_id=ACCESS_KEY
aws_secret_access_key=SECRET=KEY=WITH=EQUALS
aws_session_token=SESSION_TOKEN";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result["aws_secret_access_key"].Should().Be("SECRET=KEY=WITH=EQUALS");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithProfileSection_ShouldIgnoreSection()
    {
        // Arrange
        var rawCreds = @"[profile production]
aws_access_key_id=PROD_ACCESS
aws_secret_access_key=PROD_SECRET
aws_session_token=PROD_TOKEN";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainKey("aws_access_key_id");
        result.Should().NotContainKey("[profile production]");
    }

    #endregion

    #region GetAwsCredentialsDict - Invalid Scenarios

    [Fact]
    public void GetAwsCredentialsDict_WithNullInput_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string rawCreds = null!;

        // Act
        Action act = () => AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("AWS_CREDENTIALS is missing required keys.");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithEmptyInput_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var rawCreds = "";

        // Act
        Action act = () => AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("AWS_CREDENTIALS is missing required keys.");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithOnlyTwoKeys_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var rawCreds = @"aws_access_key_id=ACCESS_KEY
aws_secret_access_key=SECRET_KEY";

        // Act
        Action act = () => AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("AWS_CREDENTIALS is missing required keys.");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithOnlyOneKey_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var rawCreds = "aws_access_key_id=ACCESS_KEY";

        // Act
        Action act = () => AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetAwsCredentialsDict_WithOnlySectionHeaders_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var rawCreds = @"[default]
[profile dev]
[profile prod]";

        // Act
        Action act = () => AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetAwsCredentialsDict_WithInvalidLines_ShouldIgnoreThem()
    {
        // Arrange - lines without = should be ignored
        var rawCreds = @"aws_access_key_id=ACCESS_KEY
aws_secret_access_key=SECRET_KEY
aws_session_token=SESSION_TOKEN
invalid_line_without_equals";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result.Should().HaveCount(3);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void GetAwsCredentialsDict_WithSpecialCharactersInValue_ShouldPreserveValue()
    {
        // Arrange
        var rawCreds = @"aws_access_key_id=ACCESS_KEY!@#$%
aws_secret_access_key=SECRET/KEY+WITH+SPECIAL
aws_session_token=TOKEN~`123";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result["aws_access_key_id"].Should().Be("ACCESS_KEY!@#$%");
        result["aws_secret_access_key"].Should().Be("SECRET/KEY+WITH+SPECIAL");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithCarriageReturnLineFeeds_ShouldHandleCorrectly()
    {
        // Arrange - Windows-style line endings
        var rawCreds = "aws_access_key_id=ACCESS\r\naws_secret_access_key=SECRET\r\naws_session_token=TOKEN";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result.Should().HaveCount(3);
        result["aws_access_key_id"].Should().Be("ACCESS");
    }

    [Fact]
    public void GetAwsCredentialsDict_WithMoreThanThreeKeys_ShouldReturnAllKeys()
    {
        // Arrange
        var rawCreds = @"aws_access_key_id=ACCESS
aws_secret_access_key=SECRET
aws_session_token=TOKEN
extra_key=EXTRA_VALUE";

        // Act
        var result = AwsCredentialsUtils.GetAwsCredentialsDict(rawCreds);

        // Assert
        result.Should().HaveCount(4);
        result.Should().ContainKey("extra_key");
    }

    #endregion
}
