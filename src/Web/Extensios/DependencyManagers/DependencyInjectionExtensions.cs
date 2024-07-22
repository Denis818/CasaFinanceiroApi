using Application.Interfaces.Services.Finance.Comandos;
using Application.Interfaces.Services.Finance.Consultas;
using Application.Interfaces.Services.User;
using Application.Interfaces.Utilities;
using Application.Services.Finance.Comandos;
using Application.Services.Finance.Consultas;
using Application.Services.User;
using Application.Utilities;
using Data.Repository.Categorias;
using Data.Repository.Despesas;
using Data.Repository.User;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Despesa;
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

        public static void AddDependecyComandoServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthAppService, AuthAppService>();

            services.AddScoped<ICategoriaComandoServices, CategoriaComandoServices>();
            services.AddScoped<IDespesaComandoService, DespesaComandoService>();
            services.AddScoped<IGrupoFaturaComandoService, GrupoFaturaComandoService>();
            services.AddScoped<IMembroComandoServices, MembroComandoServices>();
        }

        public static void AddDependecyConsultaServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoriaConsultaServices, CategoriaConsultaServices>();
            services.AddScoped<IGrupoFaturaConsultaService, GrupoFaturaConsultaService>();
            services.AddScoped<IMembroConsultaServices, MembroConsultaServices>();

            services.AddScoped<IDashboardConsultaServices, DashboardConsultaServices>();
            services.AddScoped<IPainelControleConsultaServices, PainelControleConsultaServices>();
            services.AddScoped<IConferenciaComprasConsultaServices, ConferenciaComprasConsultaServices>();
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
