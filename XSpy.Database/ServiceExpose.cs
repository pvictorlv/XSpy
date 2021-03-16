using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                        settings.MigrationsAssembly("CFCExams");
                    });
#if DEBUG

                connBuilder.UseLoggerFactory(MyLoggerFactory);
#endif
                connBuilder.ConfigureWarnings(w => w.Log(RelationalEventId.MultipleCollectionIncludeWarning));
            });

            serviceCollection
                .AddScoped<UserService>()
                .AddScoped<RoleService>()
                .AddScoped<UserSettingsService>()
                .AddScoped<StateService>()
                .AddScoped<FacialDetectionService>()
                .AddScoped<ExamService>()
                .AddScoped<QuestionService>()
                .AddScoped<LogService>()
                .AddScoped<SettingsService>()
                .AddScoped<ExamReportService>()
                .AddScoped<ExamInconsistencyService>();

            serviceCollection.AddScoped<NotificationsService>();

            return serviceCollection;
        }
    }
}
