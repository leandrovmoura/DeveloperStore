namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale item is cancelled
/// </summary>
public class ItemCancelledEvent
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CancelledAt { get; set; }
}