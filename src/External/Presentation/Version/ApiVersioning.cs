using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
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

        public static void ConfigureWebApiVersions(this WebApplicationBuilder builder)
        {
            var presentationAssembly = typeof(MainController).Assembly;

            builder.Services
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

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddApiVersioning(setup =>
            {
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
                setup.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }
    }
}
