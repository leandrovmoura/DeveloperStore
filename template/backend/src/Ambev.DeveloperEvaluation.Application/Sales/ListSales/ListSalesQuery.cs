using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Query for listing all sales
/// </summary>
public class ListSalesQuery : IRequest<List<ListSalesResult>>
{
    public bool IncludeCancelled { get; set; }
}