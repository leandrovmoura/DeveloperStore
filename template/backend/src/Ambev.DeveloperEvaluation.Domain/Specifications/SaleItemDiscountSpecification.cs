using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

/// <summary>
/// Specification to validate sale item discount rules
/// </summary>
public class SaleItemDiscountSpecification : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item)
    {
        // Below 4 items: no discount allowed
        if (item.Quantity < 4)
        {
            return item.Discount == 0;
        }

        // 4-9 items: 10% discount
        if (item.Quantity >= 4 && item.Quantity < 10)
        {
            return item.Discount == 0.10m;
        }

        // 10-20 items: 20% discount
        if (item.Quantity >= 10 && item.Quantity <= 20)
        {
            return item.Discount == 0.20m;
        }

        // Above 20 items: not allowed
        return false;
    }
}