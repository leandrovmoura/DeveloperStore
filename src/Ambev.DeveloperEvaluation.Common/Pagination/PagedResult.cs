namespace Ambev.DeveloperEvaluation.Common.Pagination;

/// <summary>
/// Response model for paginated results
/// </summary>
/// <typeparam name="T">Type of items in the page</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Gets or sets the items in the current page
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets the index of the first item on this page (1-based)
    /// </summary>
    public int FirstItemOnPage => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Gets the index of the last item on this page (1-based)
    /// </summary>
    public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalCount);
}