using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (existingSale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");

        var oldTotalAmount = existingSale.TotalAmount;

        // Update sale properties
        existingSale.CustomerId = command.CustomerId;
        existingSale.CustomerName = command.CustomerName;
        existingSale.BranchId = command.BranchId;
        existingSale.BranchName = command.BranchName;
        existingSale.UpdatedAt = DateTime.UtcNow;

        // Update items
        existingSale.Items.Clear();
        foreach (var itemCommand in command.Items)
        {
            var item = new SaleItem
            {
                Id = itemCommand.Id ?? Guid.NewGuid(),
                SaleId = existingSale.Id,
                ProductId = itemCommand.ProductId,
                ProductName = itemCommand.ProductName,
                Quantity = itemCommand.Quantity,
                UnitPrice = itemCommand.UnitPrice
            };

            item.CalculateDiscount();
            item.CalculateTotalAmount();
            existingSale.Items.Add(item);
        }

        // Recalculate total
        existingSale.CalculateTotalAmount();

        // Validate entity
        var entityValidation = existingSale.Validate();
        if (!entityValidation.IsValid)
        {
            throw new InvalidOperationException(
    $"Sale validation failed: {string.Join(", ", entityValidation.Errors.Select(e => e.Detail))}");
        }

        // Update in repository
        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        // Publish SaleModified event
        var saleModifiedEvent = new SaleModifiedEvent
        {
            SaleId = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            OldTotalAmount = oldTotalAmount,
            NewTotalAmount = updatedSale.TotalAmount,
            ModifiedAt = updatedSale.UpdatedAt ?? DateTime.UtcNow
        };

        _logger.LogInformation(
            "SaleModified Event: SaleId={SaleId}, SaleNumber={SaleNumber}, OldTotalAmount={OldTotalAmount}, NewTotalAmount={NewTotalAmount}",
            saleModifiedEvent.SaleId,
            saleModifiedEvent.SaleNumber,
            saleModifiedEvent.OldTotalAmount,
            saleModifiedEvent.NewTotalAmount);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}