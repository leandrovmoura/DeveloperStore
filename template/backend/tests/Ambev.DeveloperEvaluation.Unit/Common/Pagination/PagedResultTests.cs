using Ambev.DeveloperEvaluation.Common.Pagination;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Pagination;

/// <summary>
/// Unit tests for PagedResult model
/// </summary>
public class PagedResultTests
{
    [Fact]
    public void TotalPages_WithExactDivision_ShouldCalculateCorrectly()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 100,
            PageSize = 10
        };

        // Act & Assert
        Assert.Equal(10, pagedResult.TotalPages);
    }

    [Fact]
    public void TotalPages_WithRemainder_ShouldRoundUp()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 105,
            PageSize = 10
        };

        // Act & Assert
        Assert.Equal(11, pagedResult.TotalPages);
    }

    [Fact]
    public void TotalPages_WithZeroCount_ShouldReturnZero()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 0,
            PageSize = 10
        };

        // Act & Assert
        Assert.Equal(0, pagedResult.TotalPages);
    }

    [Fact]
    public void HasPreviousPage_OnFirstPage_ShouldReturnFalse()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 100
        };

        // Act & Assert
        Assert.False(pagedResult.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_OnSecondPage_ShouldReturnTrue()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 2,
            PageSize = 10,
            TotalCount = 100
        };

        // Act & Assert
        Assert.True(pagedResult.HasPreviousPage);
    }

    [Fact]
    public void HasNextPage_OnLastPage_ShouldReturnFalse()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 10,
            PageSize = 10,
            TotalCount = 100
        };

        // Act & Assert
        Assert.False(pagedResult.HasNextPage);
    }

    [Fact]
    public void HasNextPage_NotOnLastPage_ShouldReturnTrue()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 5,
            PageSize = 10,
            TotalCount = 100
        };

        // Act & Assert
        Assert.True(pagedResult.HasNextPage);
    }

    [Fact]
    public void FirstItemOnPage_OnFirstPage_ShouldBe1()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 100
        };

        // Act & Assert
        Assert.Equal(1, pagedResult.FirstItemOnPage);
    }

    [Fact]
    public void FirstItemOnPage_OnSecondPage_ShouldBe11()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 2,
            PageSize = 10,
            TotalCount = 100
        };

        // Act & Assert
        Assert.Equal(11, pagedResult.FirstItemOnPage);
    }

    [Fact]
    public void LastItemOnPage_OnFullPage_ShouldBePageSize()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 100
        };

        // Act & Assert
        Assert.Equal(10, pagedResult.LastItemOnPage);
    }

    [Fact]
    public void LastItemOnPage_OnPartialLastPage_ShouldBeTotalCount()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 11,
            PageSize = 10,
            TotalCount = 105
        };

        // Act & Assert
        Assert.Equal(105, pagedResult.LastItemOnPage);
    }

    [Fact]
    public void FirstItemOnPage_WithZeroCount_ShouldBeZero()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0
        };

        // Act & Assert
        Assert.Equal(0, pagedResult.FirstItemOnPage);
    }
}