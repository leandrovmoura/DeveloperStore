using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for Sale entity
/// </summary>
public class SaleTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var sale = new Sale();

        // Assert
        Assert.NotEqual(DateTime.MinValue, sale.SaleDate);
        Assert.NotEqual(DateTime.MinValue, sale.CreatedAt);
        Assert.False(sale.IsCancelled);
        Assert.Empty(sale.Items);
        Assert.Equal(0, sale.TotalAmount);
    }

    [Fact]
    public void CalculateTotalAmount_ShouldSumAllNonCancelledItems()
    {
        // Arrange
        var sale = new Sale
        {
            Items = new List<SaleItem>
            {
                new SaleItem { Quantity = 5, UnitPrice = 100, Discount = 0.10m },
                new SaleItem { Quantity = 10, UnitPrice = 50, Discount = 0.20m },
                new SaleItem { Quantity = 3, UnitPrice = 20, Discount = 0, IsCancelled = true }
            }
        };

        foreach (var item in sale.Items)
        {
            item.CalculateTotalAmount();
        }

        // Act
        sale.CalculateTotalAmount();

        // Assert
        // Item 1: 5 * 100 - (5 * 100 * 0.10) = 500 - 50 = 450
        // Item 2: 10 * 50 - (10 * 50 * 0.20) = 500 - 100 = 400
        // Item 3: Cancelled, should not be included
        Assert.Equal(850, sale.TotalAmount);
    }

    [Fact]
    public void Cancel_ShouldSetIsCancelledAndCancelledAt()
    {
        // Arrange
        var sale = new Sale();
        var beforeCancel = DateTime.UtcNow;

        // Act
        sale.Cancel();
        var afterCancel = DateTime.UtcNow;

        // Assert
        Assert.True(sale.IsCancelled);
        Assert.NotNull(sale.CancelledAt);
        Assert.InRange(sale.CancelledAt.Value, beforeCancel, afterCancel);
        Assert.NotNull(sale.UpdatedAt);
    }

    [Fact]
    public void Cancel_ShouldCascadeCancellationToAllItems()
    {
        // Arrange
        var alreadyCancelledTime = DateTime.UtcNow.AddHours(-1); // Cancelled 1 hour ago

        var sale = new Sale
        {
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-001",
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100,
                    IsCancelled = false
                },
                new SaleItem
                {
                    ProductId = "PROD-002",
                    ProductName = "Product B",
                    Quantity = 10,
                    UnitPrice = 50,
                    IsCancelled = false
                },
                new SaleItem
                {
                    ProductId = "PROD-003",
                    ProductName = "Product C",
                    Quantity = 3,
                    UnitPrice = 20,
                    IsCancelled = true,              // Already cancelled
                    CancelledAt = alreadyCancelledTime  // With timestamp
                }
            }
        };

        var beforeCancel = DateTime.UtcNow;

        // Act
        sale.Cancel();
        var afterCancel = DateTime.UtcNow;

        // Assert
        Assert.True(sale.IsCancelled);
        Assert.NotNull(sale.CancelledAt);
        Assert.InRange(sale.CancelledAt.Value, beforeCancel, afterCancel);

        // All items should be cancelled
        Assert.All(sale.Items, item => Assert.True(item.IsCancelled));

        // Verify newly cancelled items have timestamps within expected range
        Assert.NotNull(sale.Items[0].CancelledAt);
        Assert.InRange(sale.Items[0].CancelledAt.Value, beforeCancel, afterCancel);

        Assert.NotNull(sale.Items[1].CancelledAt);
        Assert.InRange(sale.Items[1].CancelledAt.Value, beforeCancel, afterCancel);

        // Already cancelled item should keep its original timestamp
        Assert.NotNull(sale.Items[2].CancelledAt);
        Assert.Equal(alreadyCancelledTime, sale.Items[2].CancelledAt.Value);
    }

    [Fact]
    public void Validate_WithValidData_ShouldReturnIsValid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "John Doe",
            BranchId = "BRANCH-001",
            BranchName = "Main Branch",
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-001",
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100,
                    Discount = 0.10m
                }
            }
        };

        foreach (var item in sale.Items)
        {
            item.CalculateDiscount();
            item.CalculateTotalAmount();
        }

        // Act
        var result = sale.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithEmptySaleNumber_ShouldReturnInvalid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "John Doe",
            BranchId = "BRANCH-001",
            BranchName = "Main Branch",
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-001",
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100
                }
            }
        };

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_WithNoItems_ShouldReturnInvalid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "John Doe",
            BranchId = "BRANCH-001",
            BranchName = "Main Branch",
            Items = new List<SaleItem>()
        };

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail.Contains("at least one item"));
    }
}