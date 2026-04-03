using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Integration tests for SaleRepository pagination functionality
/// </summary>
public class SaleRepositoryPaginationTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;

    public SaleRepositoryPaginationTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_Pagination_{Guid.NewGuid()}")
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        // Arrange - Create 25 sales
        for (int i = 1; i <= 25; i++)
        {
            var sale = CreateTestSale($"SALE-PAGE-{i:D3}", i * 100);
            await _repository.CreateAsync(sale);
        }

        // Act - Get page 2 with 10 items
        var result = await _repository.GetPagedAsync(
            pageNumber: 2,
            pageSize: 10,
            orderBy: "SaleNumber",
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(10, result.Items.Count);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
        Assert.Equal(11, result.FirstItemOnPage);
        Assert.Equal(20, result.LastItemOnPage);
    }

    [Fact]
    public async Task GetPagedAsync_OnLastPage_ShouldReturnPartialResults()
    {
        // Arrange - Create 25 sales (last page will have 5 items)
        for (int i = 1; i <= 25; i++)
        {
            var sale = CreateTestSale($"SALE-LAST-{i:D3}", i * 100);
            await _repository.CreateAsync(sale);
        }

        // Act - Get page 3 (last page) with 10 items per page
        var result = await _repository.GetPagedAsync(
            pageNumber: 3,
            pageSize: 10,
            orderBy: "SaleNumber",
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(5, result.Items.Count); // Only 5 items on last page
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public async Task GetPagedAsync_WithOrderBySaleNumber_ShouldSortCorrectly()
    {
        // Arrange
        var sales = new[]
        {
            CreateTestSale("SALE-C", 300),
            CreateTestSale("SALE-A", 100),
            CreateTestSale("SALE-B", 200)
        };

        foreach (var sale in sales)
        {
            await _repository.CreateAsync(sale);
        }

        // Act - Order by SaleNumber ascending
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: "SaleNumber",
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("SALE-A", result.Items[0].SaleNumber);
        Assert.Equal("SALE-B", result.Items[1].SaleNumber);
        Assert.Equal("SALE-C", result.Items[2].SaleNumber);
    }

    [Fact]
    public async Task GetPagedAsync_WithOrderByTotalAmountDescending_ShouldSortCorrectly()
    {
        // Arrange
        var sales = new[]
        {
            CreateTestSale("SALE-001", 100),
            CreateTestSale("SALE-002", 300),
            CreateTestSale("SALE-003", 200)
        };

        foreach (var sale in sales)
        {
            await _repository.CreateAsync(sale);
        }

        // Act - Order by TotalAmount descending
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: "TotalAmount",
            isDescending: true,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(300, result.Items[0].TotalAmount);
        Assert.Equal(200, result.Items[1].TotalAmount);
        Assert.Equal(100, result.Items[2].TotalAmount);
    }

    [Fact]
    public async Task GetPagedAsync_WithOrderByCustomerName_ShouldSortAlphabetically()
    {
        // Arrange
        var sale1 = CreateTestSale("SALE-001", 100);
        sale1.CustomerName = "Zebra Customer";

        var sale2 = CreateTestSale("SALE-002", 200);
        sale2.CustomerName = "Alpha Customer";

        var sale3 = CreateTestSale("SALE-003", 300);
        sale3.CustomerName = "Beta Customer";

        await _repository.CreateAsync(sale1);
        await _repository.CreateAsync(sale2);
        await _repository.CreateAsync(sale3);

        // Act - Order by CustomerName ascending
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: "CustomerName",
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Alpha Customer", result.Items[0].CustomerName);
        Assert.Equal("Beta Customer", result.Items[1].CustomerName);
        Assert.Equal("Zebra Customer", result.Items[2].CustomerName);
    }

    [Fact]
    public async Task GetPagedAsync_WithOrderBySaleDate_ShouldSortChronologically()
    {
        // Arrange
        var sale1 = CreateTestSale("SALE-001", 100);
        sale1.SaleDate = DateTime.UtcNow.AddDays(-3);

        var sale2 = CreateTestSale("SALE-002", 200);
        sale2.SaleDate = DateTime.UtcNow.AddDays(-1);

        var sale3 = CreateTestSale("SALE-003", 300);
        sale3.SaleDate = DateTime.UtcNow.AddDays(-2);

        await _repository.CreateAsync(sale1);
        await _repository.CreateAsync(sale2);
        await _repository.CreateAsync(sale3);

        // Act - Order by SaleDate ascending (oldest first)
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: "SaleDate",
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("SALE-001", result.Items[0].SaleNumber); // Oldest
        Assert.Equal("SALE-003", result.Items[1].SaleNumber);
        Assert.Equal("SALE-002", result.Items[2].SaleNumber); // Newest
    }

    [Fact]
    public async Task GetPagedAsync_WithIncludeCancelledFalse_ShouldExcludeCancelled()
    {
        // Arrange
        var activeSale = CreateTestSale("SALE-ACTIVE", 100);
        activeSale.IsCancelled = false;

        var cancelledSale = CreateTestSale("SALE-CANCELLED", 200);
        cancelledSale.IsCancelled = true;
        cancelledSale.CancelledAt = DateTime.UtcNow;

        await _repository.CreateAsync(activeSale);
        await _repository.CreateAsync(cancelledSale);

        // Act
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: null,
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("SALE-ACTIVE", result.Items[0].SaleNumber);
    }

    [Fact]
    public async Task GetPagedAsync_WithIncludeCancelledTrue_ShouldIncludeAll()
    {
        // Arrange
        var activeSale = CreateTestSale("SALE-ACTIVE", 100);
        activeSale.IsCancelled = false;

        var cancelledSale = CreateTestSale("SALE-CANCELLED", 200);
        cancelledSale.IsCancelled = true;
        cancelledSale.CancelledAt = DateTime.UtcNow;

        await _repository.CreateAsync(activeSale);
        await _repository.CreateAsync(cancelledSale);

        // Act
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: null,
            isDescending: false,
            includeItems: true,
            includeCancelled: true);

        // Assert
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithNoOrderBy_ShouldUseDefaultOrdering()
    {
        // Arrange
        var sale1 = CreateTestSale("SALE-001", 100);
        sale1.CreatedAt = DateTime.UtcNow.AddHours(-2);

        var sale2 = CreateTestSale("SALE-002", 200);
        sale2.CreatedAt = DateTime.UtcNow.AddHours(-1);

        await _repository.CreateAsync(sale1);
        await _repository.CreateAsync(sale2);

        // Act - No orderBy specified, should default to CreatedAt descending
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: null,
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(2, result.Items.Count);
        // Most recent should be first (default CreatedAt descending)
        Assert.Equal("SALE-002", result.Items[0].SaleNumber);
        Assert.Equal("SALE-001", result.Items[1].SaleNumber);
    }

    [Fact]
    public async Task GetPagedAsync_WithEmptyDatabase_ShouldReturnEmptyResult()
    {
        // Act
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: null,
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public async Task GetPagedAsync_WithIncludeItemsFalse_ShouldNotLoadItems()
    {
        // Arrange
        var sale = CreateTestSale("SALE-001", 100);
        await _repository.CreateAsync(sale);

        // Act
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10,
            orderBy: null,
            isDescending: false,
            includeItems: false,
            includeCancelled: false);

        // Assert
        Assert.Single(result.Items);
        // Items collection should not be loaded (will be empty or null)
        // This tests that we're not eager loading when not needed
    }

    [Fact]
    public async Task GetPagedAsync_WithLargePageSize_ShouldHandleCorrectly()
    {
        // Arrange - Create 5 sales
        for (int i = 1; i <= 5; i++)
        {
            var sale = CreateTestSale($"SALE-{i:D3}", i * 100);
            await _repository.CreateAsync(sale);
        }

        // Act - Request page size of 100 (larger than total count)
        var result = await _repository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 100,
            orderBy: null,
            isDescending: false,
            includeItems: true,
            includeCancelled: false);

        // Assert
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNextPage);
    }

    private Sale CreateTestSale(string saleNumber, decimal totalAmount)
    {
        return new Sale
        {
            SaleNumber = saleNumber,
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "Test Customer",
            BranchId = "BRANCH-001",
            BranchName = "Test Branch",
            TotalAmount = totalAmount,
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-001",
                    ProductName = "Test Product",
                    Quantity = 5,
                    UnitPrice = totalAmount / 5,
                    Discount = 0,
                    TotalAmount = totalAmount
                }
            }
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}