namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Result for cancel sale item operation
/// </summary>
public class CancelSaleItemResult
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public decimal NewSaleTotalAmount { get; set; }
}