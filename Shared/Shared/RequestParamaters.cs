namespace Shared.Shared;
public abstract class RequestParameters
{
    private const int MaxPageSize = 50; // The max amount of resources per page.
    public int PageNumber { get; set; } = 1; // The page number starts in 1 by default
    private int _pageSize = 10; // the default page size is 10

    public int PageSize // This property act like a filter to the page size field.
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}

