using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace API.Utils;

public static class HttpContextExtension
{
    public static async Task EnterParametersPaginationInHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        if (httpContext is null) throw new ArgumentNullException(nameof(httpContext));

        double count = await queryable.CountAsync();
        httpContext.Response.Headers.Add("TotalRegisters", count.ToString(CultureInfo.InvariantCulture));
    }
}