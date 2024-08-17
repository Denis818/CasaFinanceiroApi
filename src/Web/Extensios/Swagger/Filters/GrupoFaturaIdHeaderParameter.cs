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

            if (
                (controllerPrecisaIdGrupo || endPointPrecisaIdGrupo)
                && context.ApiDescription.HttpMethod == "GET"
            )
            {
                operation.Parameters ??= [];

                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "grupo-fatura-code",
                        In = ParameterLocation.Header,
                        Description =
                            "Adicionar o Code de um grupo de despesas no cabeçalho da requisição",
                        Required = false,
                        Schema = new OpenApiSchema { Type = "int" }
                    }
                );
            }
        }
    }
}
