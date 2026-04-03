using Ambev.DeveloperEvaluation.Common.Pagination;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Query for listing all sales
/// </summary>
public class ListSalesQuery : IRequest<PagedResult<ListSalesResult>>
{
    public int PageNumber { get; set; } = 1;          
    public int PageSize { get; set; } = 10;           
    public string? OrderBy { get; set; }              
    public bool IsDescending { get; set; } = false;  
    public bool IncludeCancelled { get; set; } = false;
}