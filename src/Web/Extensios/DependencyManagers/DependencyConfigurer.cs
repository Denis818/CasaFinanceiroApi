﻿using Application.Commands.Validators.Finance;
using FluentValidation;
using Infraestructure.Data.Configurations.DataBaseConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Web.Extensios.DependencyManagers
{
    public static class DependencyConfigurer
    {
        public static void AddApiDependencyServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddDependecyUtilities();
            services.AddDependecyRepositories();
            services.AddDependecyDomainServices();
            services.AddDependecyCommandsServices();
            services.AddDependecyQueriesServices();

            services.AddAuthenticationJwt(config);
            services.AddAssemblyConfigurations();

            services.AddCompanyConnectionStrings(config);
            services.AddDbContext(config);

            services.AddDependecyMiddlewares();
        }

        public static void AddAuthenticationJwt(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            var key = Encoding.ASCII.GetBytes(config["Jwt:key"]);
            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = config["TokenConfiguration:Issuer"],
                        ValidAudience = config["TokenConfiguration:Audience"]
                    };
                });
        }

        public static void AddAssemblyConfigurations(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(DespesaValidator)));
        }

        public static void UseCorsPolicy(
            this IApplicationBuilder app,
            IWebHostEnvironment _environmentHost
        )
        {
            if (!_environmentHost.IsDevelopment())
            {
                app.UseCors(builder =>
                    builder
                        .WithOrigins(
                            "https://casa-financeiro-dev.netlify.app",
                            "https://casa-financeiro-app.netlify.app"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            }
            else
            {
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            }
        }
    }
}
