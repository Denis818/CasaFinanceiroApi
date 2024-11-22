using Data.DataContext;
using Domain.Enumeradores;
using Domain.Utilities;
using Infraestructure.Data.Configurations;
using Presentation.Api.Base;
using System.Text.Json;

namespace Web.Middleware
{
    public class IdentificadorDataBaseMiddleware(
        FinanceDbContext dbContext,
        CompanyConnectionStrings companyConnections
    ) : IMiddleware
    {
        private readonly FinanceDbContext _context = dbContext;
        private readonly CompanyConnectionStrings _companyConnections = companyConnections;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var connectionString = await IdentificarStringConexao(context);

            _context.SetConnectionString(connectionString);

            await next(context);
        }

        public async Task<string> IdentificarStringConexao(HttpContext context)
        {
            string origin = context.Request.Headers["Origin"].ToString();

            string hostName = null;

            if (string.IsNullOrEmpty(origin))
            {
                origin = $"https://{context.Request.Host}";
            }

            if (!string.IsNullOrEmpty(origin))
            {
                var originUri = new Uri(origin);
                hostName = originUri.Host;
            }

            var empresaLocalizada = _companyConnections.List.FirstOrDefault(empresa =>
                empresa.NomeDominio == hostName
            );

            if (empresaLocalizada == null)
            {
                var response = new ResponseDTO<string>
                {
                    Mensagens = [
                        new Notificacao($"A empresa com nome de domínio '{hostName}' não foi encontrada",
                            EnumTipoNotificacao.NotFount)
                        ]
                };

                context.Response.Headers.TryAdd("content-type", "application/json; charset=utf-8");
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return null;
            }

            return empresaLocalizada.ConnectionString;
        }
    }
}
