using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        sale.Cancel();
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Publish SaleCancelled event
        var saleCancelledEvent = new SaleCancelledEvent
        {
            SaleId = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            TotalAmount = updatedSale.TotalAmount,
            CancelledAt = updatedSale.CancelledAt ?? DateTime.UtcNow
        };

        _logger.LogInformation(
            "SaleCancelled Event: SaleId={SaleId}, SaleNumber={SaleNumber}, TotalAmount={TotalAmount}",
            saleCancelledEvent.SaleId,
            saleCancelledEvent.SaleNumber,
            saleCancelledEvent.TotalAmount);

        return _mapper.Map<CancelSaleResult>(updatedSale);
    }
}