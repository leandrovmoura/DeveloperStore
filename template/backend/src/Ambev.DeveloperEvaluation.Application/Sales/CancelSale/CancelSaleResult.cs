namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Result for cancel sale operation
/// </summary>
public class CancelSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
}