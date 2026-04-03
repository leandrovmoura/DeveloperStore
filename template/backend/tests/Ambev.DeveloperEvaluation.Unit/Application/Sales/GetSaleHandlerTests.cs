using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Unit tests for GetSaleHandler
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WithExistingSale_ShouldReturnSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "SALE-001",
            CustomerName = "John Doe"
        };

        var expectedResult = new GetSaleResult
        {
            Id = saleId,
            SaleNumber = "SALE-001"
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        var query = new GetSaleQuery(saleId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.SaleNumber, result.SaleNumber);
    }

    [Fact]
    public async Task Handle_WithNonExistingSale_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var query = new GetSaleQuery(saleId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));
    }
}