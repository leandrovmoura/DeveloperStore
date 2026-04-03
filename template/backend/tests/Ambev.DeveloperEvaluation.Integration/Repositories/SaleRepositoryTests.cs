using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Integration tests for SaleRepository
/// </summary>
public class SaleRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;

    public SaleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddSaleToDatabase()
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
                    Discount = 0.10m,
                    TotalAmount = 450
                }
            },
            TotalAmount = 450
        };

        // Act
        var result = await _repository.CreateAsync(sale);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Single(result.Items);
        
        var savedSale = await _context.Sales.Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(savedSale);
        Assert.Equal("SALE-001", savedSale.SaleNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingSale_ShouldReturnSaleWithItems()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-002",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-002",
            CustomerName = "Jane Doe",
            BranchId = "BRANCH-002",
            BranchName = "Downtown Branch",
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-002",
                    ProductName = "Product B",
                    Quantity = 10,
                    UnitPrice = 50,
                    Discount = 0.20m,
                    TotalAmount = 400
                }
            },
            TotalAmount = 400
        };

        await _repository.CreateAsync(sale);

        // Act
        var result = await _repository.GetByIdAsync(sale.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("SALE-002", result.SaleNumber);
        Assert.Single(result.Items);
        Assert.Equal("Product B", result.Items.First().ProductName);
    }

    [Fact]
    public async Task GetBySaleNumberAsync_WithExistingSaleNumber_ShouldReturnSale()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-003",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-003",
            CustomerName = "Bob Smith",
            BranchId = "BRANCH-003",
            BranchName = "Uptown Branch",
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-003",
                    ProductName = "Product C",
                    Quantity = 4,
                    UnitPrice = 25,
                    Discount = 0.10m,
                    TotalAmount = 90
                }
            },
            TotalAmount = 90
        };

        await _repository.CreateAsync(sale);

        // Act
        var result = await _repository.GetBySaleNumberAsync("SALE-003");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sale.Id, result.Id);
        Assert.Equal("Bob Smith", result.CustomerName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSaleInDatabase()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-004",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-004",
            CustomerName = "Alice Johnson",
            BranchId = "BRANCH-004",
            BranchName = "Suburban Branch",
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-004",
                    ProductName = "Product D",
                    Quantity = 5,
                    UnitPrice = 30,
                    Discount = 0.10m,
                    TotalAmount = 135
                }
            },
            TotalAmount = 135
        };

        await _repository.CreateAsync(sale);

        // Act
        sale.CustomerName = "Alice Updated";
        sale.UpdatedAt = DateTime.UtcNow;
        var result = await _repository.UpdateAsync(sale);

        // Assert
        Assert.Equal("Alice Updated", result.CustomerName);
        Assert.NotNull(result.UpdatedAt);

        var updatedSale = await _context.Sales.FindAsync(sale.Id);
        Assert.NotNull(updatedSale);
        Assert.Equal("Alice Updated", updatedSale.CustomerName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSaleFromDatabase()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-005",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-005",
            CustomerName = "Charlie Brown",
            BranchId = "BRANCH-005",
            BranchName = "City Branch",
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = "PROD-005",
                    ProductName = "Product E",
                    Quantity = 8,
                    UnitPrice = 40,
                    Discount = 0.10m,
                    TotalAmount = 288
                }
            },
            TotalAmount = 288
        };

        await _repository.CreateAsync(sale);

        // Act
        var result = await _repository.DeleteAsync(sale.Id);

        // Assert
        Assert.True(result);
        var deletedSale = await _context.Sales.FindAsync(sale.Id);
        Assert.Null(deletedSale);
    }

    [Fact]
    public async Task GetAllAsync_WithIncludeCancelledFalse_ShouldReturnOnlyActiveSales()
    {
        // Arrange
        var activeSale = new Sale
        {
            SaleNumber = "SALE-006",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-006",
            CustomerName = "David Lee",
            BranchId = "BRANCH-006",
            BranchName = "Branch 6",
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new SaleItem { ProductId = "PROD-006", ProductName = "Product F", Quantity = 5, UnitPrice = 20 }
            }
        };

        var cancelledSale = new Sale
        {
            SaleNumber = "SALE-007",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-007",
            CustomerName = "Eve Martinez",
            BranchId = "BRANCH-007",
            BranchName = "Branch 7",
            IsCancelled = true,
            Items = new List<SaleItem>
            {
                new SaleItem { ProductId = "PROD-007", ProductName = "Product G", Quantity = 3, UnitPrice = 15 }
            }
        };

        await _repository.CreateAsync(activeSale);
        await _repository.CreateAsync(cancelledSale);

        // Act
        var result = await _repository.GetAllAsync(includeItems: true, includeCancelled: false);

        // Assert
        Assert.Single(result);
        Assert.Equal("SALE-006", result.First().SaleNumber);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}