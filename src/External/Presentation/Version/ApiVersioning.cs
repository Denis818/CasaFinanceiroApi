using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Api.Base;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Presentation.Version
{
    public static class ApiVersioning
    {
        public const string V1 = "1.0";
        public const string V2 = "2.0";

        public static readonly string[] ListVersions = [V1, V2];

        public static void ConfigureWebApi(this IServiceCollection services)
        {
            var presentationAssembly = typeof(MainController).Assembly;

            services
                .AddControllers(options =>
                {
                    options.Conventions.Add(new ApiVersioningFilter());
                })
                .AddApplicationPart(presentationAssembly)
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddApiVersioning(setup =>
            {
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
                setup.DefaultApiVersion = new ApiVersion(1, 0);
            });
            //.AddApiExplorer(options =>
            //{
            //    options.SubstituteApiVersionInUrl = true;
            //    options.GroupNameFormat = "'v'VVV";
            //});
        }
    }
}
