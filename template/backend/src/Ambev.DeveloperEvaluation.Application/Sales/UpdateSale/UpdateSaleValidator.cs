using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleCommand
/// </summary>
public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sale ID is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters.");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch ID is required.");

        RuleFor(x => x.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Sale must have at least one item.");

        RuleForEach(x => x.Items)
            .SetValidator(new UpdateSaleItemValidator());
    }
}

public class UpdateSaleItemValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0.");
    }
}