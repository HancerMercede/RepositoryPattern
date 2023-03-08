using Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Repository;

namespace RepositoryPatternArquitecture.Helpers;

public static class ServiceExtensions
{
    public static void ConfiguredCors(this IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy("AllowAll", builder =>
               builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
        });
    }
    public static void ConfiguredSqlContext(this IServiceCollection services, string connection)
    {
        services.AddDbContext<RepositoryContext>(opts =>
        opts.UseSqlServer(connection));
    }

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
}
