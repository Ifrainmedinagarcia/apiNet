using API.DTOs;

namespace API.Utils;

public static class IQueryableExtensions
{
    public static IQueryable<T> Pagination<T>(this IQueryable<T> queryable, PaginationDTo paginationDTo)
    {
        return queryable
            .Skip((paginationDTo.Page - 1) * paginationDTo.RecordsPerPage)
            .Take(paginationDTo.RecordsPerPage);
    }
}