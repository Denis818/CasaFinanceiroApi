using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Asp.Versioning;
using Domain.Enumeradores;
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
        IGrupoFaturaCommandService _grupoFaturaComandoService) : MainController(service)
    {
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<bool> PostAsync(GrupoFaturaCommandDto grupoFaturaDto) =>
            await _grupoFaturaComandoService.InsertAsync(grupoFaturaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<bool> PutAsync(Guid code, GrupoFaturaCommandDto grupoFaturaDto) =>
            await _grupoFaturaComandoService.UpdateAsync(code, grupoFaturaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(Guid code) => await _grupoFaturaComandoService.DeleteAsync(code);

        [HttpPut("status-fatura")]
        [GetIdGroupInHeaderFilter]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<bool> PutStatusFaturaAsync(
            EnumFaturaTipo faturaNome,
            EnumStatusFatura status
        ) => await _grupoFaturaComandoService.UpdateStatusFaturaAsync(faturaNome, status);
    }
}
