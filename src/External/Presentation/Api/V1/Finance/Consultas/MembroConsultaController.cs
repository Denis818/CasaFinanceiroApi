using Application.Queries.Interfaces;
using Asp.Versioning;
using Domain.Dtos;
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
    public class MembroConsultaController(IServiceProvider service, IMembroQueryServices _membroConsultaServices)
        : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<MembroDto>> GetAllDespesaAsync() =>
            await _membroConsultaServices.GetAllAsync();

        [HttpGet("enviar-mensagem")]
        [GetIdGroupInHeaderFilter]
        public async Task<object> EnviarValoresDividosPeloWhatsAppAsync(
            string nome,
            string pix,
            bool isMoradia,
            string titleMessage
        )
        {
            var messagemWhastApp = await _membroConsultaServices.EnviarValoresDividosPeloWhatsAppAsync(
                nome,
                titleMessage,
                isMoradia,
                pix
            );

            return new { RedirectToWhatsApp = messagemWhastApp };
        }
    }
}
