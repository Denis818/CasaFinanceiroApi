using Microsoft.OpenApi.Models;
using Presentation.Attributes.Util;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.Extensios.Swagger.Filters
{
    public class GrupoFaturaIdHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerPrecisaIdGrupo = context
                .MethodInfo.DeclaringType.GetCustomAttributes(true)
                .OfType<GetIdGroupInHeaderFilterAttribute>()
                .Any();

            var endPointPrecisaIdGrupo = context
                .MethodInfo.GetCustomAttributes(true)
                .OfType<GetIdGroupInHeaderFilterAttribute>()
                .Any();

            var controllerIgnoraIdGrupo = context
                .MethodInfo.DeclaringType.GetCustomAttributes(true)
                .OfType<IgnoreGrupoIdAttribute>()
                .Any();

            var endPointIgnoraIdGrupo = context
                .MethodInfo.GetCustomAttributes(true)
                .OfType<IgnoreGrupoIdAttribute>()
                .Any();

            if (
                (controllerPrecisaIdGrupo || endPointPrecisaIdGrupo)
                && !controllerIgnoraIdGrupo
                && !endPointIgnoraIdGrupo
                && context.ApiDescription.HttpMethod == "GET"
            )
            {
                operation.Parameters ??= new List<OpenApiParameter>();

                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "grupo-fatura-code",
                        In = ParameterLocation.Header,
                        Required = false,
                        Schema = new OpenApiSchema { Type = "string" },
                        Description = "Adicionar o Code de um grupo de despesas no cabeçalho da requisição",
                    }
                );
            }
        }
    }
}
