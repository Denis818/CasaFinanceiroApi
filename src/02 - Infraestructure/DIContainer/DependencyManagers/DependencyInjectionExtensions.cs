﻿using Application.Interfaces.Services.Categorias;
using Application.Interfaces.Services.Despesas;
using Application.Interfaces.Services.Membros;
using Application.Interfaces.Services.User;
using Application.Interfaces.Utilities;
using Application.Services.Categorias;
using Application.Services.Despesas;
using Application.Services.Membros;
using Application.Services.User;
using Application.Utilities;
using Data.Repository.Base.Membros;
using Data.Repository.Categorias;
using Data.Repository.Despesas;
using Data.Repository.User;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyUtilities(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IMapper, Mapper>();
            services.AddScoped<INotifier, Notifier>();
        }

        public static void AddDependecyRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IDespesaRepository, DespesaRepository>();
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        }

        public static void AddDependecyAppServices(this IServiceCollection services)
        {

            services.AddScoped<IAuthAppService, AuthAppService>();
            services.AddScoped<IDespesaAppServices, DespesaAppServices>();
            services.AddScoped<IMembroAppServices, MembroAppServices>();
            services.AddScoped<ICategoriaAppServices, CategoriaAppServices>();

            services.AddScoped<IDespesaConsultaAppService, DespesaConsultaAppService>();
        }
    }
}