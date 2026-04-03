using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional;

/// <summary>
/// Functional tests for Sales API endpoints
/// </summary>
public class SalesControllerTests : IClassFixture<WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program> _factory;

    public SalesControllerTests(WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateSale_WithValidData_ShouldReturn201Created()
    {
        // Arrange
        var request = new CreateSaleRequest
        {
            SaleNumber = $"SALE-{Guid.NewGuid()}",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "John Doe",
            BranchId = "BRANCH-001",
            BranchName = "Main Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest
                {
                    ProductId = "PROD-001",
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("SALE-", content);
    }

    [Fact]
    public async Task CreateSale_WithInvalidData_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new CreateSaleRequest
        {
            SaleNumber = "", // Invalid
            Items = new List<CreateSaleItemRequest>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSale_WithExistingSale_ShouldReturn200OK()
    {
        // Arrange - First create a sale
        var createRequest = new CreateSaleRequest
        {
            SaleNumber = $"SALE-{Guid.NewGuid()}",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-002",
            CustomerName = "Jane Doe",
            BranchId = "BRANCH-002",
            BranchName = "Downtown Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest
                {
                    ProductId = "PROD-002",
                    ProductName = "Product B",
                    Quantity = 10,
                    UnitPrice = 50
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(createContent);
        var saleId = jsonDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        // Act
        var response = await _client.GetAsync($"/api/sales/{saleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Jane Doe", content);
    }

    [Fact]
    public async Task GetSale_WithNonExistingSale_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/sales/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListSales_ShouldReturn200OK()
    {
        // Act
        var response = await _client.GetAsync("/api/sales");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CancelSale_WithActiveSale_ShouldReturn200OK()
    {
        // Arrange - Create a sale first
        var createRequest = new CreateSaleRequest
        {
            SaleNumber = $"SALE-{Guid.NewGuid()}",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-003",
            CustomerName = "Bob Smith",
            BranchId = "BRANCH-003",
            BranchName = "Uptown Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest
                {
                    ProductId = "PROD-003",
                    ProductName = "Product C",
                    Quantity = 4,
                    UnitPrice = 25
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(createContent);
        var saleId = jsonDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        // Act
        var response = await _client.PostAsync($"/api/sales/{saleId}/cancel", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSale_WithExistingSale_ShouldReturn200OK()
    {
        // Arrange - Create a sale first
        var createRequest = new CreateSaleRequest
        {
            SaleNumber = $"SALE-{Guid.NewGuid()}",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-004",
            CustomerName = "Alice Johnson",
            BranchId = "BRANCH-004",
            BranchName = "Suburban Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest
                {
                    ProductId = "PROD-004",
                    ProductName = "Product D",
                    Quantity = 5,
                    UnitPrice = 30
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(createContent);
        var saleId = jsonDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/sales/{saleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}