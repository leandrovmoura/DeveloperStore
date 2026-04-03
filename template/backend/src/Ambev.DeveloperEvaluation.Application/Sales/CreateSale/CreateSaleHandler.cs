using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Check if sale number already exists
        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        // Map command to entity
        var sale = _mapper.Map<Sale>(command);

        // Apply business rules: Calculate discount and totals for each item
        foreach (var item in sale.Items)
        {
            item.CalculateDiscount();
            item.CalculateTotalAmount();
        }

        // Calculate total amount
        sale.CalculateTotalAmount();

        // Validate entity
        var entityValidation = sale.Validate();
        if (!entityValidation.IsValid)
        {
            throw new InvalidOperationException(
                $"Sale validation failed: {string.Join(", ", entityValidation.Errors.Select(e => e.Detail))}");
        }

        // Save to repository
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Publish SaleCreated event
        var saleCreatedEvent = new SaleCreatedEvent
        {
            SaleId = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            SaleDate = createdSale.SaleDate,
            CustomerId = createdSale.CustomerId,
            CustomerName = createdSale.CustomerName,
            BranchId = createdSale.BranchId,
            BranchName = createdSale.BranchName,
            TotalAmount = createdSale.TotalAmount,
            ItemCount = createdSale.Items.Count,
            CreatedAt = createdSale.CreatedAt
        };

        _logger.LogInformation(
            "SaleCreated Event: SaleId={SaleId}, SaleNumber={SaleNumber}, CustomerId={CustomerId}, TotalAmount={TotalAmount}, ItemCount={ItemCount}",
            saleCreatedEvent.SaleId,
            saleCreatedEvent.SaleNumber,
            saleCreatedEvent.CustomerId,
            saleCreatedEvent.TotalAmount,
            saleCreatedEvent.ItemCount);

        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}