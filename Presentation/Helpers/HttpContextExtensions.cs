using System.Globalization;
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
        
        httpContext.Response.Headers.Append("TotalRecords", count.ToString(CultureInfo.InvariantCulture));
       
        httpContext.Response.Headers.Append("PaginationData", JsonSerializer.Serialize(metaData));

    }
}
