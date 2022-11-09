using DataTablesTest.Data;
using Microsoft.EntityFrameworkCore;

namespace DataTablesTest.Services;

public static class ServiceCollectionExtensions
{
    public static void RegisterDataServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ClassicModelsContext>
        (options => options
            .UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.Parse("10.4.24-mariadb"),
                option => option.EnableRetryOnFailure())
        );
    }
}