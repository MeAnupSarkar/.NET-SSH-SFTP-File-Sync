using MediaFon.FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MediaFon.FileManager.Infrastructure;

public static  class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return services.AddDbContext<ApplicationDbContext>(option => option.UseNpgsql(config.GetConnectionString("PostgreSqlContext")));
    }
}
