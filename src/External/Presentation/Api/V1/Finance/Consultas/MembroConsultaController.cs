using Application.Interfaces.Services.Membros;
using Asp.Versioning;
using Domain.Models.Membros;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.Consultas
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [ApiVersionRoute("membro")]
    public class MembroCrudController(IServiceProvider service, IMembroAppServices _membroServices)
        : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<Membro>> GetAllDespesaAsync() =>
            await _membroServices.GetAllAsync();

        [HttpGet("enviar-mensagem")]
        [GetIdGroupInHeaderFilter]
        public async Task<object> EnviarValoresDividosPeloWhatsAppAsync(
            string nome,
            string pix,
            bool isMoradia,
            string titleMessage
        )
        {
            var messagemWhastApp = await _membroServices.EnviarValoresDividosPeloWhatsAppAsync(
                nome,
                titleMessage,
                isMoradia,
                pix
            );

            return new { RedirectToWhatsApp = messagemWhastApp };
        }
    }
}
