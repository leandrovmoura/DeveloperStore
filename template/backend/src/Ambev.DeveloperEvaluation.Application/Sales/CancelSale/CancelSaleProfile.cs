using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// AutoMapper profile for CancelSale operations
/// </summary>
public class CancelSaleProfile : Profile
{
    public CancelSaleProfile()
    {
        CreateMap<Sale, CancelSaleResult>();
    }
}