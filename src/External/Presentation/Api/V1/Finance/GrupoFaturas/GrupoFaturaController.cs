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

namespace Presentation.Api.V1.Finance.GrupoFaturas
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [Route("api/v1/grupo-fatura")]
    public class GrupoFaturaController(
        IServiceProvider service,
        IGrupoFaturaAppService _grupoFaturaServices,
        IStatusFaturaAppService _statusFaturaServices
    ) : MainController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<IEnumerable<GrupoFatura>> GetAllAsync() =>
            await _grupoFaturaServices.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<string> GetNameFatura(int id) =>
            await _grupoFaturaServices.GetNameFatura(id);

        [HttpPost]
        [ClearCache]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<GrupoFatura> PostAsync(GrupoFaturaDto grupoFaturaDto) =>
            await _grupoFaturaServices.InsertAsync(grupoFaturaDto);

        [HttpPut]
        [ClearCache]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<GrupoFatura> PutAsync(int id, GrupoFaturaDto grupoFaturaDto) =>
            await _grupoFaturaServices.UpdateAsync(id, grupoFaturaDto);

        [HttpDelete]
        [ClearCache]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _grupoFaturaServices.DeleteAsync(id);
        #endregion

        #region Status Fatura

        [HttpGet("status-fatura")]
        [GetIdGroupInHeaderFilter]
        public async Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status) =>
            await _statusFaturaServices.GetStatusFaturaDtoByNameAsync(status);

        [HttpPut("status-fatura")]
        [GetIdGroupInHeaderFilter]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<StatusFatura> PutStatusFaturaAsync(
            EnumFaturaTipo faturaNome,
            EnumStatusFatura status
        ) => await _statusFaturaServices.UpdateAsync(faturaNome, status);

        #endregion
    }
}
