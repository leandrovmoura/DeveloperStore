namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale is modified
/// </summary>
public class SaleModifiedEvent
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public decimal OldTotalAmount { get; set; }
    public decimal NewTotalAmount { get; set; }
    public DateTime ModifiedAt { get; set; }
}