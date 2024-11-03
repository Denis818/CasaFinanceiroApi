using Application.Commands.Interfaces;
using Application.Commands.Services;
using Application.Commands.Services.ListaCompras;
using Application.Queries.Interfaces;
using Application.Queries.Interfaces.Despesas;
using Application.Queries.Interfaces.ListaCompras;
using Application.Queries.Interfaces.Telas;
using Application.Queries.Services;
using Application.Queries.Services.Despesas;
using Application.Queries.Services.ListaCompras;
using Application.Queries.Services.Telas;
using Application.Utilities;
using Data.Repository.Categorias;
using Data.Repository.Despesas;
using Data.Repository.User;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Repositories.Cobrancas;
using Domain.Interfaces.Repositories.GrupoFaturas;
using Domain.Interfaces.Repositories.ListaCompras;
using Domain.Interfaces.Repositories.Membros;
using Domain.Interfaces.Repositories.Permissoes;
using Domain.Interfaces.Repositories.Users;
using Domain.Interfaces.Services.Despesa;
using Domain.Interfaces.Utilities;
using Domain.Services;
using Infraestructure.Data.Configurations;
using Infraestructure.Data.Repository;
using Infraestructure.Data.Repository.Cobrancas;
using Infraestructure.Data.Repository.ListaCompras;
using Infraestructure.Data.Repository.Membros;
using Infraestructure.Data.Repository.Permissoes;
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
            services.AddScoped<IPermissaoRepository, PermissaoRepository>();
            services.AddScoped<IDespesaRepository, DespesaRepository>();
            services.AddScoped<IProdutoListaComprasRepository, ProdutoListaComprasRepository>();
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IGrupoFaturaRepository, GrupoFaturaRepository>();
            services.AddScoped<IStatusFaturaRepository, StatusFaturaRepository>();
            services.AddScoped<IParametroDeAlertaDeGastosRepository, ParametroDeAlertaDeGastosRepository>();
            services.AddScoped<ICobrancaRepository, CobrancaRepository>();
            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
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
            services.AddScoped<IProdutoListaComprasCommandService, ProdutoListaComprasCommandService>();
            services.AddScoped<IGrupoFaturaCommandService, GrupoFaturaCommandService>();
            services.AddScoped<IMembroComandoServices, MembroCommandServices>();
            services.AddScoped<IParametroDeAlertaDeGastosCommandService, ParametroDeAlertaDeGastosCommandService>();
        }

        public static void AddDependecyQueriesServices(this IServiceCollection services)
        {
            services.AddScoped<IDespesaFiltroService, DespesaFiltroService>();
            services.AddScoped<ICategoriaQueryServices, CategoriaQueryServices>();
            services.AddScoped<IGrupoFaturaQueryService, GrupoFaturaQueryService>();
            services.AddScoped<IProdutoListaComprasQueryService, ProdutoListaComprasQueryService>();
            services.AddScoped<IMembroQueryServices, MembroQueryServices>();

            services.AddScoped<IDashboardQueryServices, DashboardQueryServices>();
            services.AddScoped<IPainelControleQueryServices, PainelControleQueryServices>();
            services.AddScoped<IAuditoriaComprasQueryServices, AuditoriaComprasQueryServices>();
            services.AddScoped<IComparativoFaturasQueryServices, ComparativoFaturasQueryServices>();
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
