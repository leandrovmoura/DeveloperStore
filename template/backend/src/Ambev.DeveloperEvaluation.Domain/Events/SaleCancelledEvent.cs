namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale is cancelled
/// </summary>
public class SaleCancelledEvent
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CancelledAt { get; set; }
}