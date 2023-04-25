using Dtos.DtoModels;
using Entities.Models;
using Mapster;
using System.Reflection;

namespace RepositoryPatternArquitecture.Configurations;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<Company, CompanyDto>
        .NewConfig()
        .Map(dest => dest.FullAddress, src => string.Concat(src.Address, " ", src.Country));
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
    }
}
