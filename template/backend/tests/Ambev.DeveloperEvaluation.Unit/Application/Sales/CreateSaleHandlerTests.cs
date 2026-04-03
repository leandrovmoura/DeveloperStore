using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Unit tests for CreateSaleHandler
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateSale()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "John Doe",
            BranchId = "BRANCH-001",
            BranchName = "Main Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = "PROD-001",
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100
                }
            }
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = "PROD-001",
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100
                }
            }
        };

        var expectedResult = new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateSaleNumber_ShouldThrowException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "John Doe",
            BranchId = "BRANCH-001",
            BranchName = "Main Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = "PROD-001",
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100
                }
            }
        };

        var existingSale = new Sale { SaleNumber = command.SaleNumber };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "", // Invalid
            Items = new List<CreateSaleItemCommand>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}