using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Command for cancelling a sale item
/// </summary>
public class CancelSaleItemCommand : IRequest<CancelSaleItemResult>
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
}