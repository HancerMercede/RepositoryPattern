using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace RepositoryPatternArquictecture.Helpers;

public static class ServiceExtensions
{
    public static void ConfiguredSqlContext(this IServiceCollection services, string connection)
    {
        services.AddDbContext<RepositoryContext>(opts =>
        opts.UseSqlServer(connection));
    }
}
