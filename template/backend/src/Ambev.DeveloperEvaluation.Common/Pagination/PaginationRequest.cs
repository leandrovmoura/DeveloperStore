namespace Ambev.DeveloperEvaluation.Common.Pagination;

/// <summary>
/// Base request model for paginated queries
/// </summary>
public class PaginationRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Gets or sets the page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size (max 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Gets or sets the field to sort by
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets whether to sort in descending order (default: false = ascending)
    /// </summary>
    public bool IsDescending { get; set; } = false;
}