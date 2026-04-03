using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

/// <summary>
/// Specification to validate sale item quantity against business rules
/// </summary>
public class SaleItemQuantitySpecification : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item)
    {
        return item.Quantity > 0 && item.Quantity <= 20;
    }
}