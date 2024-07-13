using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Data.Configurations.DataBaseConfiguration
{
    public static class SeedUser
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionStrings = config
                .GetSection("CompanyConnectionStrings:List")
                .Get<List<CompanyInfo>>();

            foreach (var company in connectionStrings)
            {
                services.AddDbContext<FinanceDbContext>(options =>
                    options.UseMySql(
                        company.ConnectionString,
                        ServerVersion.AutoDetect(company.ConnectionString)
                    )
                );

                using var scope = services.BuildServiceProvider().CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

                dbContext.Database.SetConnectionString(company.ConnectionString);
                dbContext.Database.Migrate();

                PrepareDataBaseExtensions.PrepareDataBase(
                    scope.ServiceProvider,
                    company.NomeDominio
                );
            }
        }
    }
}
