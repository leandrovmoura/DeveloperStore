using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

/// <summary>
/// Unit tests for SaleItemDiscountSpecification
/// </summary>
public class SaleItemDiscountSpecificationTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    public void IsSatisfiedBy_WithQuantityBelow4AndNoDiscount_ShouldReturnTrue(int quantity, decimal discount)
    {
        // Arrange
        var specification = new SaleItemDiscountSpecification();
        var item = new SaleItem { Quantity = quantity, Discount = discount };

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(4, 0.10)]
    [InlineData(5, 0.10)]
    [InlineData(9, 0.10)]
    public void IsSatisfiedBy_WithQuantity4To9And10PercentDiscount_ShouldReturnTrue(int quantity, decimal discount)
    {
        // Arrange
        var specification = new SaleItemDiscountSpecification();
        var item = new SaleItem { Quantity = quantity, Discount = discount };

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(10, 0.20)]
    [InlineData(15, 0.20)]
    [InlineData(20, 0.20)]
    public void IsSatisfiedBy_WithQuantity10To20And20PercentDiscount_ShouldReturnTrue(int quantity, decimal discount)
    {
        // Arrange
        var specification = new SaleItemDiscountSpecification();
        var item = new SaleItem { Quantity = quantity, Discount = discount };

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WithQuantityBelow4AndDiscount_ShouldReturnFalse()
    {
        // Arrange
        var specification = new SaleItemDiscountSpecification();
        var item = new SaleItem { Quantity = 3, Discount = 0.10m };

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_WithWrongDiscountTier_ShouldReturnFalse()
    {
        // Arrange
        var specification = new SaleItemDiscountSpecification();
        var item = new SaleItem { Quantity = 5, Discount = 0.20m }; // Should be 0.10

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_WithQuantityAbove20_ShouldReturnFalse()
    {
        // Arrange
        var specification = new SaleItemDiscountSpecification();
        var item = new SaleItem { Quantity = 21, Discount = 0 };

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        Assert.False(result);
    }
}