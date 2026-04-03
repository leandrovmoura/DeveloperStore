using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Query for retrieving a sale by ID
/// </summary>
public class GetSaleQuery : IRequest<GetSaleResult>
{
    public Guid Id { get; set; }

    public GetSaleQuery(Guid id)
    {
        Id = id;
    }
}