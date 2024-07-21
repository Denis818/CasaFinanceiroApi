using Application.Interfaces.Services.Finance.Consultas;
using Asp.Versioning;
using Domain.Dtos.Despesas.Criacao;
using Domain.Models.Despesas;
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
        IGrupoFaturaConsultaService _grupoFaturaConsultaService) : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<GrupoFatura>> GetAllAsync(string ano) =>
            await _grupoFaturaConsultaService.GetAllAsync(ano);

        [HttpGet("{id}")]
        public async Task<string> GetNameFatura(int id) =>
            await _grupoFaturaConsultaService.GetNameFatura(id);

        [HttpGet("status-fatura")]
        [GetIdGroupInHeaderFilter]
        public async Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status) =>
            await _grupoFaturaConsultaService.GetStatusFaturaDtoByNameAsync(status);
    }
}
