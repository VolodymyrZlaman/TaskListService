namespace TaskService.Domain.Common;

/// <summary>
/// Domain model for paginated results
/// </summary>
/// <typeparam name="T">Type of elements in the collection</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Collection of items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; }
    
    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalCount { get; }
    
    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; }
    
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    private PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
    
    /// <summary>
    /// Creates a new instance of paginated result
    /// </summary>
    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
} 