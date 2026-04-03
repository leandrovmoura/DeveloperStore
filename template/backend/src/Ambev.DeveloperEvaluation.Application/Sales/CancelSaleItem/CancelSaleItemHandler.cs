using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handler for processing CancelSaleItemCommand
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.SaleId} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot cancel items from a cancelled sale");

        var item = sale.Items.FirstOrDefault(i => i.Id == command.ItemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {command.ItemId} not found in sale");

        if (item.IsCancelled)
            throw new InvalidOperationException("Item is already cancelled");

        item.Cancel();
        sale.CalculateTotalAmount();
        sale.UpdatedAt = DateTime.UtcNow;

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish ItemCancelled event
        var itemCancelledEvent = new ItemCancelledEvent
        {
            SaleId = command.SaleId,
            ItemId = command.ItemId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            TotalAmount = item.TotalAmount,
            CancelledAt = item.CancelledAt ?? DateTime.UtcNow
        };

        _logger.LogInformation(
            "ItemCancelled Event: SaleId={SaleId}, ItemId={ItemId}, ProductId={ProductId}, ProductName={ProductName}",
            itemCancelledEvent.SaleId,
            itemCancelledEvent.ItemId,
            itemCancelledEvent.ProductId,
            itemCancelledEvent.ProductName);

        return new CancelSaleItemResult
        {
            SaleId = command.SaleId,
            ItemId = command.ItemId,
            IsCancelled = true,
            CancelledAt = item.CancelledAt,
            NewSaleTotalAmount = updatedSale.TotalAmount
        };
    }
}