using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_DefaultValues_AreCorrect()
    {
        // Act
        var response = new ApiResponse<string>();

        // Assert
        Assert.False(response.Success);
        Assert.Null(response.Data);
        Assert.Equal(string.Empty, response.Message);
        Assert.Null(response.Error);
    }

    [Fact]
    public void ApiResponse_SuccessResponse_PropertiesSetCorrectly()
    {
        // Arrange
        var data = new List<string> { "test1", "test2" };
        var message = "Success message";

        // Act
        var response = new ApiResponse<List<string>>
        {
            Success = true,
            Data = data,
            Message = message
        };

        // Assert
        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Equal(message, response.Message);
        Assert.Null(response.Error);
    }

    [Fact]
    public void ApiResponse_ErrorResponse_PropertiesSetCorrectly()
    {
        // Arrange
        var error = "Error message";
        var message = "Request failed";

        // Act
        var response = new ApiResponse<object>
        {
            Success = false,
            Error = error,
            Message = message
        };

        // Assert
        Assert.False(response.Success);
        Assert.Null(response.Data);
        Assert.Equal(message, response.Message);
        Assert.Equal(error, response.Error);
    }

    [Fact]
    public void ApiResponse_GenericType_WorksWithDifferentTypes()
    {
        // Arrange & Act
        var stringResponse = new ApiResponse<string> { Data = "test" };
        var intResponse = new ApiResponse<int> { Data = 42 };
        var listResponse = new ApiResponse<List<Episode>> { Data = new List<Episode>() };

        // Assert
        Assert.Equal("test", stringResponse.Data);
        Assert.Equal(42, intResponse.Data);
        Assert.NotNull(listResponse.Data);
        Assert.Empty(listResponse.Data);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ApiResponse_SuccessProperty_AcceptsBooleanValues(bool success)
    {
        // Act
        var response = new ApiResponse<object> { Success = success };

        // Assert
        Assert.Equal(success, response.Success);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Success")]
    [InlineData("Error occurred")]
    [InlineData(null)]
    public void ApiResponse_MessageProperty_AcceptsStringValues(string message)
    {
        // Act
        var response = new ApiResponse<object> { Message = message ?? string.Empty };

        // Assert
        Assert.Equal(message ?? string.Empty, response.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Validation error")]
    [InlineData("Server error")]
    public void ApiResponse_ErrorProperty_AcceptsNullableStringValues(string error)
    {
        // Act
        var response = new ApiResponse<object> { Error = error };

        // Assert
        Assert.Equal(error, response.Error);
    }
}