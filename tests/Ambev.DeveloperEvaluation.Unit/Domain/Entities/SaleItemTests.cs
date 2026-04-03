using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for SaleItem entity
/// </summary>
public class SaleItemTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var item = new SaleItem();

        // Assert
        Assert.False(item.IsCancelled);
        Assert.Equal(0, item.Quantity);
        Assert.Equal(0, item.UnitPrice);
        Assert.Equal(0, item.Discount);
        Assert.Equal(0, item.TotalAmount);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    public void CalculateDiscount_WithQuantityBelow4_ShouldHaveNoDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = new SaleItem { Quantity = quantity };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
    }

    [Theory]
    [InlineData(4, 0.10)]
    [InlineData(5, 0.10)]
    [InlineData(9, 0.10)]
    public void CalculateDiscount_WithQuantity4To9_ShouldHave10PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = new SaleItem { Quantity = quantity };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
    }

    [Theory]
    [InlineData(10, 0.20)]
    [InlineData(15, 0.20)]
    [InlineData(20, 0.20)]
    public void CalculateDiscount_WithQuantity10To20_ShouldHave20PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = new SaleItem { Quantity = quantity };

        // Act
        item.CalculateDiscount();

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
    }

    [Fact]
    public void CalculateTotalAmount_ShouldApplyDiscountCorrectly()
    {
        // Arrange
        var item = new SaleItem
        {
            Quantity = 10,
            UnitPrice = 100,
            Discount = 0.20m
        };

        // Act
        item.CalculateTotalAmount();

        // Assert
        // 10 * 100 = 1000
        // 1000 * 0.20 = 200 (discount)
        // 1000 - 200 = 800
        Assert.Equal(800, item.TotalAmount);
    }

    [Fact]
    public void Cancel_ShouldSetIsCancelledAndCancelledAt()
    {
        // Arrange
        var item = new SaleItem();
        var beforeCancel = DateTime.UtcNow;

        // Act
        item.Cancel();
        var afterCancel = DateTime.UtcNow;

        // Assert
        Assert.True(item.IsCancelled);
        Assert.NotNull(item.CancelledAt);
        Assert.InRange(item.CancelledAt.Value, beforeCancel, afterCancel);
    }

    [Fact]
    public void Validate_WithValidData_ShouldReturnIsValid()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = "PROD-001",
            ProductName = "Product A",
            Quantity = 5,
            UnitPrice = 100,
            Discount = 0.10m
        };

        // Act
        var result = item.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithQuantityAbove20_ShouldReturnInvalid()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = "PROD-001",
            ProductName = "Product A",
            Quantity = 21,
            UnitPrice = 100,
            Discount = 0
        };

        // Act
        var result = item.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail.Contains("20"));
    }

    [Fact]
    public void Validate_WithDiscountOnLessThan4Items_ShouldReturnInvalid()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = "PROD-001",
            ProductName = "Product A",
            Quantity = 3,
            UnitPrice = 100,
            Discount = 0.10m
        };

        // Act
        var result = item.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail.Contains("below 4 items"));
    }
}