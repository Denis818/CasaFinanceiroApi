using Application.Commands.Interfaces;
using Application.Commands.Services;
using Application.Queries.Interfaces;
using Application.Queries.Services;
using Application.Utilities;
using Data.Repository.Categorias;
using Data.Repository.Despesas;
using Data.Repository.User;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Despesa;
using Domain.Interfaces.Utilities;
using Domain.Services;
using Infraestructure.Data.Configurations;
using Infraestructure.Data.Repository.Membros;
using Presentation.ModelState;
using Presentation.ModelState.Interface;
using Web.Middleware;

namespace Web.Extensios.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyUtilities(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<INotifier, Notifier>();
            services.AddScoped<IModelStateValidator, ModelStateValidator>();
        }

        public static void AddDependecyRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IDespesaRepository, DespesaRepository>();
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IGrupoFaturaRepository, GrupoFaturaRepository>();
            services.AddScoped<IStatusFaturaRepository, StatusFaturaRepository>();
        }

        public static void AddDependecyDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IDespesaDomainServices, DespesaDomainServices>();
        }

        public static void AddDependecyCommandsServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthCommandService, AuthCommandService>();

            services.AddScoped<ICategoriaCommandServices, CategoriaCommandServices>();
            services.AddScoped<IDespesaCommandService, DespesaCommandService>();
            services.AddScoped<IGrupoFaturaCommandService, GrupoFaturaCommandService>();
            services.AddScoped<IMembroComandoServices, MembroCommandServices>();
        }

        public static void AddDependecyQueriesServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoriaQueryServices, CategoriaQueryServices>();
            services.AddScoped<IGrupoFaturaQueryService, GrupoFaturaQueryService>();
            services.AddScoped<IMembroQueryServices, MembroQueryServices>();

            services.AddScoped<IDashboardQueryServices, DashboardQueryServices>();
            services.AddScoped<IPainelControleQueryServices, PainelControleQueryServices>();
            services.AddScoped<IConferenciaComprasQueryServices, ConferenciaComprasQueryServices>();
        }

        public static void AddCompanyConnectionStrings(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var appSettingsSection = configuration.GetSection(nameof(CompanyConnectionStrings));
            var appSettings = appSettingsSection.Get<CompanyConnectionStrings>();
            services.AddSingleton(appSettings);
        }

        public static void AddDependecyMiddlewares(this IServiceCollection services)
        {
            services.AddTransient<ExceptionMiddleware>();
            services.AddTransient<IdentificadorDataBaseMiddleware>();
        }
    }
}
