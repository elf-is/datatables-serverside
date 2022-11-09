using DataTablesTest.Data;

namespace DataTablesTest.Services;

public static class HostExtensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ClassicModelsContext>();
        context.Database.EnsureCreated();
    }
}