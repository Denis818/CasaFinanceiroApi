using Application.Resources.Messages;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation.Api.Base;

namespace Presentation.Attributes.Util
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GetIdGroupInHeaderFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var httpContext = context.HttpContext;
            string grupoId = httpContext.Request.Headers["grupo-fatura-code"];

            if (httpContext.Request.Path.ToString().Contains("total-por-grupo"))
            {
                return;
            }

            if (int.TryParse(grupoId, out int GrupoFaturasId))
            {
                httpContext.Items["grupo-fatura-code"] = GrupoFaturasId;
            }
            else if (httpContext.Request.Method == "GET")
            {
                context.Result = new BadRequestObjectResult(
                    new ResponseDTO<string>()
                    {
                        Mensagens =
                        [
                            new(Message.SelecioneUmGrupoDesesa, EnumTipoNotificacao.ClientError)
                        ]
                    }
                );
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }
}
