using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// AutoMapper profile for ListSales endpoint
/// </summary>
public class ListSalesProfile : Profile
{
    public ListSalesProfile()
    {
        CreateMap<ListSalesResult, ListSalesResponse>();
    }
}