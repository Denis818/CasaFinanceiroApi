using Domain.Enumeradores;
using Presentation.Api.Base;
using System.Text.Json;

namespace Web.Middleware
{
    public class ExceptionMiddleware(IWebHostEnvironment _environmentHost) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var message =
                    $"Erro interno no servidor. {(_environmentHost.IsDevelopment() ? ex.Message : "")}";

                var response = new ResponseDTO<string>()
                {
                    Mensagens = [new(message, EnumTipoNotificacao.ServerError)]
                };

                context.Response.Headers.Append("content-type", "application/json; charset=utf-8");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
