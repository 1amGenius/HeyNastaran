namespace Core.Contracts.Common;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items
    {
        get;
        init;
    } = Array.Empty<T>();

    public int Page
    {
        get;
        init;
    }

    public int PageSize
    {
        get;
        init;
    }

    public int TotalCount
    {
        get;
        init;
    }

    public bool HasNext
        => (Page + 1) * PageSize < TotalCount;

    public bool HasPrev
        => Page is > 0;
}
