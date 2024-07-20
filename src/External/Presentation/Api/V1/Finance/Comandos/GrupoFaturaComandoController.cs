using Application.Interfaces.Services.Despesas;
using Application.Services;
using Asp.Versioning;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Cached;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.Comandos
{
    [ClearCache]
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [ApiVersionRoute("grupo-fatura")]
    public class GrupoFaturaComandoController(
        IServiceProvider service,
        IGrupoFaturaAppService _grupoFaturaServices,
        IStatusFaturaAppService _statusFaturaServices
    ) : MainController(service)
    {
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<GrupoFatura> PostAsync(GrupoFaturaDto grupoFaturaDto) =>
            await _grupoFaturaServices.InsertAsync(grupoFaturaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<GrupoFatura> PutAsync(int id, GrupoFaturaDto grupoFaturaDto) =>
            await _grupoFaturaServices.UpdateAsync(id, grupoFaturaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _grupoFaturaServices.DeleteAsync(id);

        [HttpPut("status-fatura")]
        [GetIdGroupInHeaderFilter]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<StatusFatura> PutStatusFaturaAsync(
            EnumFaturaTipo faturaNome,
            EnumStatusFatura status
        ) => await _statusFaturaServices.UpdateAsync(faturaNome, status);
    }
}
