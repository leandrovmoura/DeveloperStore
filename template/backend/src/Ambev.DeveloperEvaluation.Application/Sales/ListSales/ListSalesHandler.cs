using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesQuery
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesQuery, List<ListSalesResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<List<ListSalesResult>> Handle(ListSalesQuery query, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(
            includeItems: true,
            includeCancelled: query.IncludeCancelled,
            cancellationToken);

        return _mapper.Map<List<ListSalesResult>>(sales);
    }
}