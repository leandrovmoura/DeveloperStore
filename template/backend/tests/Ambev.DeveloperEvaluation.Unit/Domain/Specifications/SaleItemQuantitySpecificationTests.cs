using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

/// <summary>
/// Unit tests for SaleItemQuantitySpecification
/// </summary>
public class SaleItemQuantitySpecificationTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    public void IsSatisfiedBy_WithValidQuantity_ShouldReturnTrue(int quantity)
    {
        // Arrange
        var specification = new SaleItemQuantitySpecification();
        var item = new SaleItem { Quantity = quantity };

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    [InlineData(100)]
    public void IsSatisfiedBy_WithInvalidQuantity_ShouldReturnFalse(int quantity)
    {
        // Arrange
        var specification = new SaleItemQuantitySpecification();
        var item = new SaleItem { Quantity = quantity };

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.False(result);
    }
}