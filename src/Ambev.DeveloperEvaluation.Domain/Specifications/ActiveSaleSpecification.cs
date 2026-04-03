using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

/// <summary>
/// Specification to check if a sale is active (not cancelled)
/// </summary>
public class ActiveSaleSpecification : ISpecification<Sale>
{
    public bool IsSatisfiedBy(Sale sale)
    {
        return !sale.IsCancelled;
    }
}