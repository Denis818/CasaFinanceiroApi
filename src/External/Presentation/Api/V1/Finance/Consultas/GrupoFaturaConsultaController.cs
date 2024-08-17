using Application.Queries.Dtos;
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
    [ApiVersionRoute("grupo-fatura")]
    public class GrupoFaturaConsultaController(
        IServiceProvider service,
        IGrupoFaturaQueryService _grupoFaturaConsultaService) : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<GrupoFaturaQueryDto>> GetAllAsync(string ano) =>
            await _grupoFaturaConsultaService.GetAllAsync(ano);

        [HttpGet("{id}")]
        public async Task<string> GetNameFatura(Guid code) =>
            await _grupoFaturaConsultaService.GetNameFatura(code);

        [HttpGet("status-fatura")]
        [GetIdGroupInHeaderFilter]
        public async Task<StatusFaturaQueryDto> GetStatusFaturaDtoByNameAsync(string status) =>
            await _grupoFaturaConsultaService.GetStatusFaturaDtoByNameAsync(status);

        [HttpGet("seletor-grupo-fatura")]
        public async Task<IEnumerable<GrupoFaturaSeletorQueryDto>> GetListGrupoFaturaParaSeletorAsync(string ano) =>
           await _grupoFaturaConsultaService.GetListGrupoFaturaParaSeletorAsync(ano);
    }
}
