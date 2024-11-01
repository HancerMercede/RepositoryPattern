using Microsoft.AspNetCore.Http;
using Shared.RequestFeatures;
using System.Text.Json;

namespace Presentation.Helpers;

public static class HttpContextExtensions
{
    public static void HeadersPaginationParametersInsert<T>(this HttpContext httpContext, IQueryable<T> queryable, MetaData metaData)
    {
        if (httpContext is null) { throw new ArgumentNullException(nameof(httpContext)); }
        double count = queryable.Count();
        httpContext.Response.Headers.Add("TotalRecords", count.ToString());
        // 48fca700-c53b-43a5-bdbe-08db0f9cb76e
        httpContext.Response.Headers.Add("PaginationData", JsonSerializer.Serialize(metaData).ToString());

    }
}
