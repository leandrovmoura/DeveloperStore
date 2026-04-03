namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Paginated response for list sales operation
/// </summary>
public class PagedSalesResponse
{
    /// <summary>
    /// Gets or sets the sales in the current page
    /// </summary>
    public List<ListSalesResponse> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of sales across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Gets the index of the first item on this page (1-based)
    /// </summary>
    public int FirstItemOnPage { get; set; }

    /// <summary>
    /// Gets the index of the last item on this page (1-based)
    /// </summary>
    public int LastItemOnPage { get; set; }
}