using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Pagination;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesQuery
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesQuery, PagedResult<ListSalesResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ListSalesResult>> Handle(ListSalesQuery query, CancellationToken cancellationToken)
    {
        const int maxPageSize = 100;
        var pageSize = Math.Min(query.PageSize, maxPageSize);

        var pagedSales = await _saleRepository.GetPagedAsync(
            pageNumber: query.PageNumber,
            pageSize: pageSize,
            orderBy: query.OrderBy,
            isDescending: query.IsDescending,
            includeItems: true,
            includeCancelled: query.IncludeCancelled,
            cancellationToken: cancellationToken);

        var resultItems = _mapper.Map<List<ListSalesResult>>(pagedSales.Items);

        return new PagedResult<ListSalesResult>
        {
            Items = resultItems,
            PageNumber = pagedSales.PageNumber,
            PageSize = pagedSales.PageSize,
            TotalCount = pagedSales.TotalCount
        };
    }
}