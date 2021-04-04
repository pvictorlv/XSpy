using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XSpy.Database.Services;
using XSpy.Database.Services.Users;

namespace XSpy.Database
{
    public static class ServiceExpose
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public static IServiceCollection AddDatabaseServices(this IServiceCollection serviceCollection,
            string dbConnectionString)
        {
            serviceCollection.AddDbContextPool<DatabaseContext>(builder =>
            {
                builder.EnableSensitiveDataLogging();

                var connBuilder = builder.UseMySql(
                    dbConnectionString,
                    new MariaDbServerVersion(new Version(10, 5)),
                    settings =>
                    {
                        settings.EnableRetryOnFailure(3);
                    });
#if DEBUG

                connBuilder.UseLoggerFactory(MyLoggerFactory);
#endif
                connBuilder.ConfigureWarnings(w => w.Log(RelationalEventId.MultipleCollectionIncludeWarning));
            });

            serviceCollection
                .AddScoped<UserService>()
                .AddScoped<DeviceService>()
                .AddScoped<RoleService>();

            return serviceCollection;
        }
    }
}
