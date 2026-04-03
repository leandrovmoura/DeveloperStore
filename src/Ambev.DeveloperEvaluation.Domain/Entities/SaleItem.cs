using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale.
/// This is part of the Sale aggregate.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale identifier this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier (External Identity pattern).
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name (Denormalized for query performance).
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of products sold.
    /// Must be between 1 and 20 according to business rules.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied (0-20%).
    /// Calculated based on quantity: 4-9 items = 10%, 10-20 items = 20%
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this item (quantity * unitPrice - discount).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether this item is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the item was cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Navigation property to the parent sale.
    /// </summary>
    public Sale? Sale { get; set; }

    /// <summary>
    /// Initializes a new instance of the SaleItem class.
    /// </summary>
    public SaleItem()
    {
        IsCancelled = false;
    }

    /// <summary>
    /// Calculates the discount based on quantity according to business rules.
    /// 4-9 items: 10% discount
    /// 10-20 items: 20% discount
    /// Below 4 or above 20: No discount allowed
    /// </summary>
    public void CalculateDiscount()
    {
        if (Quantity >= 10 && Quantity <= 20)
        {
            Discount = 0.20m; // 20%
        }
        else if (Quantity >= 4 && Quantity < 10)
        {
            Discount = 0.10m; // 10%
        }
        else
        {
            Discount = 0m; // No discount
        }
    }

    /// <summary>
    /// Calculates the total amount for this item including discount.
    /// </summary>
    public void CalculateTotalAmount()
    {
        var subtotal = Quantity * UnitPrice;
        var discountAmount = subtotal * Discount;
        TotalAmount = subtotal - discountAmount;
    }

    /// <summary>
    /// Cancels this item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        CancelledAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Performs validation of the sale item entity.
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
        };
    }
}