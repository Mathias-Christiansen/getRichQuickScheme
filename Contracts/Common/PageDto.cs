namespace Contracts.Common;

public class PageDto<T>
{
    public int TotalElements { get; init; } = default!;
    public int CurrentPage { get; init; } = default!;
    public int ElementsPerPage { get; init; } = default!;
    public decimal TotalPages { get; init; } = default!;
    public ICollection<T> PageElements { get; set; } = Array.Empty<T>();
}