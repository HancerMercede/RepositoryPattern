


using System.Threading.RateLimiting;
using Service;
using Service.Contracts.Interfaces;

namespace RepositoryPatternArquitecture.Helpers;

public static class ServiceExtensions
{
    public static void ConfiguredCors(this IServiceCollection services) =>
        services.AddCors(opt =>
        {
            opt.AddPolicy("AllowAll", builder =>
               builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
        });
    
    // ReSharper disable once InconsistentNaming
    public static void ConfiguredIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(opt => 
        { 
        
        });
    
    public static void ConfiguredSqlContext(this IServiceCollection services, string connection)
    {
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseSqlServer(connection));
        //opts.UseInMemoryDatabase("CompanyEmployees"));
    }

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(10),
                    }
                ));
            options.AddPolicy("genaral", ctx =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(10)
                    }
                );
            });
        });
    }

}
