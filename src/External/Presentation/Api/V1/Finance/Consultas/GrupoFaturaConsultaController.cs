using Application.Interfaces.Services.Despesas;
using Application.Services;
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
    public class GrupoFaturaCrudController(
        IServiceProvider service,
        IGrupoFaturaAppService _grupoFaturaServices,
        IStatusFaturaAppService _statusFaturaServices
    ) : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<GrupoFatura>> GetAllAsync() =>
            await _grupoFaturaServices.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<string> GetNameFatura(int id) =>
            await _grupoFaturaServices.GetNameFatura(id);

        [HttpGet("status-fatura")]
        [GetIdGroupInHeaderFilter]
        public async Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status) =>
            await _statusFaturaServices.GetStatusFaturaDtoByNameAsync(status);
    }
}
