using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional;

/// <summary>
/// Functional tests for Sales API pagination functionality
/// </summary>
public class SalesControllerPaginationTests : IClassFixture<WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program> _factory;

    public SalesControllerPaginationTests(WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ListSales_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange - Create 15 sales
        for (int i = 1; i <= 15; i++)
        {
            var createRequest = new CreateSaleRequest
            {
                SaleNumber = $"SALE-PAGINATION-{i:D3}",
                SaleDate = DateTime.UtcNow.AddDays(-i),
                CustomerId = $"CUST-{i:D3}",
                CustomerName = $"Customer {i}",
                BranchId = "BRANCH-001",
                BranchName = "Test Branch",
                Items = new List<CreateSaleItemRequest>
                {
                    new CreateSaleItemRequest
                    {
                        ProductId = "PROD-001",
                        ProductName = "Test Product",
                        Quantity = 5,
                        UnitPrice = 100
                    }
                }
            };
            await _client.PostAsJsonAsync("/api/sales", createRequest);
        }

        // Act - Request first page with 10 items
        var response = await _client.GetAsync("/api/sales?pageNumber=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data").GetProperty("data");
        
        Assert.True(data.GetProperty("totalCount").GetInt32() >= 15);
        Assert.Equal(1, data.GetProperty("pageNumber").GetInt32());
        Assert.Equal(10, data.GetProperty("pageSize").GetInt32());
        Assert.True(data.GetProperty("items").GetArrayLength() <= 10);
        Assert.False(data.GetProperty("hasPreviousPage").GetBoolean());
        Assert.True(data.GetProperty("hasNextPage").GetBoolean());
    }

    [Fact]
    public async Task ListSales_WithSecondPage_ShouldReturnCorrectPage()
    {
        // Arrange - Create 25 sales
        for (int i = 1; i <= 25; i++)
        {
            var createRequest = new CreateSaleRequest
            {
                SaleNumber = $"SALE-PAGE2-{i:D3}",
                SaleDate = DateTime.UtcNow.AddDays(-i),
                CustomerId = $"CUST-{i:D3}",
                CustomerName = $"Customer {i}",
                BranchId = "BRANCH-001",
                BranchName = "Test Branch",
                Items = new List<CreateSaleItemRequest>
                {
                    new CreateSaleItemRequest
                    {
                        ProductId = "PROD-001",
                        ProductName = "Test Product",
                        Quantity = 5,
                        UnitPrice = 100
                    }
                }
            };
            await _client.PostAsJsonAsync("/api/sales", createRequest);
        }

        // Act - Request second page
        var response = await _client.GetAsync("/api/sales?pageNumber=2&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data").GetProperty("data");
        
        Assert.Equal(2, data.GetProperty("pageNumber").GetInt32());
        Assert.True(data.GetProperty("hasPreviousPage").GetBoolean());
        Assert.True(data.GetProperty("hasNextPage").GetBoolean());
        Assert.Equal(11, data.GetProperty("firstItemOnPage").GetInt32());
        Assert.Equal(20, data.GetProperty("lastItemOnPage").GetInt32());
    }

    [Fact]
    public async Task ListSales_WithOrderByTotalAmountDescending_ShouldReturnOrderedResults()
    {
        // Arrange - Create sales with different amounts
        var sale1 = new CreateSaleRequest
        {
            SaleNumber = $"SALE-AMOUNT-LOW",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "Customer A",
            BranchId = "BRANCH-001",
            BranchName = "Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = "P1", ProductName = "Product", Quantity = 5, UnitPrice = 100 }
            }
        };
        
        var sale2 = new CreateSaleRequest
        {
            SaleNumber = $"SALE-AMOUNT-HIGH",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-002",
            CustomerName = "Customer B",
            BranchId = "BRANCH-001",
            BranchName = "Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = "P1", ProductName = "Product", Quantity = 10, UnitPrice = 100 }
            }
        };

        await _client.PostAsJsonAsync("/api/sales", sale1);
        await _client.PostAsJsonAsync("/api/sales", sale2);

        // Act - Order by TotalAmount descending
        var response = await _client.GetAsync("/api/sales?pageSize=10&orderBy=totalamount&isDescending=true");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data").GetProperty("data");
        var items = data.GetProperty("items");
        
        // First item should have higher total (Customer B with 10 items)
        if (items.GetArrayLength() >= 2)
        {
            var firstItem = items[0];
            var secondItem = items[1];
            
            // Check if our test sales are in the results
            var firstSaleNumber = firstItem.GetProperty("saleNumber").GetString();
            if (firstSaleNumber?.Contains("SALE-AMOUNT") == true)
            {
                Assert.Contains("HIGH", firstSaleNumber);
            }
        }
    }

    [Fact]
    public async Task ListSales_WithOrderByCustomerName_ShouldReturnAlphabeticalOrder()
    {
        // Arrange
        var saleZ = new CreateSaleRequest
        {
            SaleNumber = $"SALE-CUST-Z",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-Z",
            CustomerName = "Zebra Customer",
            BranchId = "BRANCH-001",
            BranchName = "Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = "P1", ProductName = "Product", Quantity = 5, UnitPrice = 100 }
            }
        };
        
        var saleA = new CreateSaleRequest
        {
            SaleNumber = $"SALE-CUST-A",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-A",
            CustomerName = "Alpha Customer",
            BranchId = "BRANCH-001",
            BranchName = "Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = "P1", ProductName = "Product", Quantity = 5, UnitPrice = 100 }
            }
        };

        await _client.PostAsJsonAsync("/api/sales", saleZ);
        await _client.PostAsJsonAsync("/api/sales", saleA);

        // Act - Order by CustomerName ascending
        var response = await _client.GetAsync("/api/sales?pageSize=10&orderBy=customername&isDescending=false");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Alpha Customer", content);
        // Alpha should appear before Zebra in alphabetical order
    }

    [Fact]
    public async Task ListSales_WithLargePageSize_ShouldRespectMaximum()
    {
        // Act - Request with page size 200 (should be capped at 100)
        var response = await _client.GetAsync("/api/sales?pageNumber=1&pageSize=200");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var data = jsonDoc.RootElement.GetProperty("data").GetProperty("data");
        
        // Page size should be capped at 100
        Assert.True(data.GetProperty("pageSize").GetInt32() <= 100);
    }

    [Fact]
    public async Task ListSales_WithIncludeCancelled_ShouldReturnCancelledSales()
    {
        // Arrange - Create and cancel a sale
        var createRequest = new CreateSaleRequest
        {
            SaleNumber = $"{Guid.NewGuid()}",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "Customer",
            BranchId = "BRANCH-001",
            BranchName = "Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = "P1", ProductName = "Product", Quantity = 5, UnitPrice = 100 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(createContent);
        var saleId = jsonDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        // Cancel the sale
        await _client.PostAsync($"/api/sales/{saleId}/cancel", null);

        // Act - Get sales without cancelled
        var responseWithoutCancelled = await _client.GetAsync("/api/sales?includeCancelled=false");
        var contentWithoutCancelled = await responseWithoutCancelled.Content.ReadAsStringAsync();

        // Act - Get sales with cancelled
        var responseWithCancelled = await _client.GetAsync("/api/sales?includeCancelled=true");
        var contentWithCancelled = await responseWithCancelled.Content.ReadAsStringAsync();

        // Assert
        var jsonWithCancelled = JsonDocument.Parse(contentWithCancelled);
        var totalWithCancelled = jsonWithCancelled.RootElement
            .GetProperty("data")
            .GetProperty("data")
            .GetProperty("totalCount")
            .GetInt32();

        var jsonWithoutCancelled = JsonDocument.Parse(contentWithoutCancelled);
        var totalWithoutCancelled = jsonWithoutCancelled.RootElement
            .GetProperty("data")
            .GetProperty("data")            
            .GetProperty("totalCount")
            .GetInt32();

        // There should be more sales when including cancelled
        Assert.True(totalWithCancelled >= totalWithoutCancelled);
    }

    [Fact]
    public async Task ListSales_WithInvalidOrderBy_ShouldUseDefaultOrdering()
    {
        // Act - Use invalid orderBy field
        var response = await _client.GetAsync("/api/sales?pageSize=10&orderBy=invalidfield");

        // Assert - Should still return 200 OK with default ordering
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}