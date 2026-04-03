using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

/// <summary>
/// Unit tests for ActiveSaleSpecification
/// </summary>
public class ActiveSaleSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_WithActiveSale_ShouldReturnTrue()
    {
        // Arrange
        var specification = new ActiveSaleSpecification();
        var sale = new Sale { IsCancelled = false };

        // Act
        var result = specification.IsSatisfiedBy(sale);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WithCancelledSale_ShouldReturnFalse()
    {
        // Arrange
        var specification = new ActiveSaleSpecification();
        var sale = new Sale { IsCancelled = true };

        // Act
        var result = specification.IsSatisfiedBy(sale);

        // Assert
        Assert.False(result);
    }
}