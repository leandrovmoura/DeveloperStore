using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for SaleItem entity with business rules
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items.");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0.");

        RuleFor(item => item.Discount)
            .InclusiveBetween(0, 0.20m).WithMessage("Discount must be between 0% and 20%.");

        // Business rule: Purchases below 4 items cannot have a discount
        RuleFor(item => item)
            .Must(item => item.Quantity >= 4 || item.Discount == 0)
            .WithMessage("Purchases below 4 items cannot have a discount.");

        // Business rule: Discount tiers must be correct
        RuleFor(item => item)
            .Must(ValidateDiscountTier)
            .WithMessage("Discount does not match the quantity tier. 4-9 items: 10%, 10-20 items: 20%.");
    }

    private bool ValidateDiscountTier(SaleItem item)
    {
        if (item.Quantity >= 10 && item.Quantity <= 20)
        {
            return item.Discount == 0.20m;
        }
        else if (item.Quantity >= 4 && item.Quantity < 10)
        {
            return item.Discount == 0.10m;
        }
        else
        {
            return item.Discount == 0m;
        }
    }
}