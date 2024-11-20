using Application.Constantes;
using AspNetCoreRateLimit;
using Presentation.Version;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Web.Extensios.DependencyManagers;
using Web.Extensios.Swagger;

namespace Web.Extensios.Application
{
    public static class AppSetupExtensions
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder)
        {
            string env = builder.Environment.EnvironmentName;
            string port = Environment.GetEnvironmentVariable("PORT") ?? "3000";

            builder.Host.UseSerilog(
                (hosting, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hosting.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(
                            outputTemplate: "[{Timestamp:HH:mm} {Level:u3}] {Message:lj}{NewLine}{Exception}\n",
                            theme: AnsiConsoleTheme.Literate
                        );
                }
            );

            builder.Configuration.AddJsonFile(
                $"appsettings.{env}.json",
                optional: false,
                reloadOnChange: true
            );

            builder.ConfigureRateLimit();
            builder.ConfigureWebApiVersions();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApiDependencyServices(builder.Configuration);
            builder.Services.AddSwaggerConfiguration();

            GetCods.GetMembersIds(builder.Services).Wait();
            GetCods.GetCategoriaCodesAsync(builder.Services).Wait();

            if (builder.Environment.IsProduction())
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }
        }

        public static void ConfigureRateLimit(this WebApplicationBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.AddMemoryCache();

            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
