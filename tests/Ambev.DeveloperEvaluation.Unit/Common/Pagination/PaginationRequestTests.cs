using Ambev.DeveloperEvaluation.Common.Pagination;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Pagination;

/// <summary>
/// Unit tests for PaginationRequest model
/// </summary>
public class PaginationRequestTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var request = new PaginationRequest();

        // Assert
        Assert.Equal(1, request.PageNumber);
        Assert.Equal(10, request.PageSize);
        Assert.Null(request.OrderBy);
        Assert.False(request.IsDescending);
    }

    [Fact]
    public void PageSize_WhenSetAbove100_ShouldCapAt100()
    {
        // Arrange
        var request = new PaginationRequest();

        // Act
        request.PageSize = 200;

        // Assert
        Assert.Equal(100, request.PageSize);
    }

    [Fact]
    public void PageSize_WhenSetTo100_ShouldBe100()
    {
        // Arrange
        var request = new PaginationRequest();

        // Act
        request.PageSize = 100;

        // Assert
        Assert.Equal(100, request.PageSize);
    }

    [Fact]
    public void PageSize_WhenSetBelow100_ShouldKeepValue()
    {
        // Arrange
        var request = new PaginationRequest();

        // Act
        request.PageSize = 25;

        // Assert
        Assert.Equal(25, request.PageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(99)]
    [InlineData(100)]
    public void PageSize_WithValidValues_ShouldAcceptValue(int pageSize)
    {
        // Arrange
        var request = new PaginationRequest();

        // Act
        request.PageSize = pageSize;

        // Assert
        Assert.Equal(pageSize, request.PageSize);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    [InlineData(1000)]
    public void PageSize_WithValuesAbove100_ShouldCapAt100(int pageSize)
    {
        // Arrange
        var request = new PaginationRequest();

        // Act
        request.PageSize = pageSize;

        // Assert
        Assert.Equal(100, request.PageSize);
    }
}