using CasaFinanceiroApi.Extensios.Swagger.Filters;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Presentation.Version;
using Swashbuckle.AspNetCore.SwaggerGen;
using Web.Extensios.Swagger.Filters;

namespace Web.Extensios.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                foreach (var version in ApiVersioning.ListVersions)
                {
                    options.SwaggerDoc(
                        $"v{version}",
                        new OpenApiInfo { Title = $"API v{version}", Version = version }
                    );
                }

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.CustomSchemaIds(x => x.FullName);

                options.AddSecuritySchema();

                options.AddSchemaFilters();

                options.MapType<EnumFiltroDespesa>(
                    () =>
                        new OpenApiSchema
                        {
                            Type = "string",
                            Enum = Enum.GetNames(typeof(EnumFiltroDespesa))
                                .Select(name => (IOpenApiAny)new OpenApiString(name))
                                .ToList()
                        }
                );
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }

        public static void AddSecuritySchema(this SwaggerGenOptions options)
        {
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = $"Insira o token JWT desta maneira \"Bearer <seu token>\" ",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { "Bearer" } }
            };

            options.AddSecurityRequirement(securityRequirement);
        }

        public static void AddSchemaFilters(this SwaggerGenOptions options)
        {
            options.SchemaFilter<DateSchemaFilter>();
            options.OperationFilter<GrupoFaturaIdHeaderParameter>();
        }

        public static void ConfigureSwaggerUI(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var version in ApiVersioning.ListVersions)
                {
                    options.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"API v{version}");
                }

                options.RoutePrefix = "doc";
                options.DocumentTitle = "Casa Financeiro API";
                options.HeadContent = File.ReadAllText(
                    Path.Combine(app.Environment.WebRootPath, "html/swagger-head.html")
                );
            });
        }
    }
}
