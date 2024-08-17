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
    [ApiVersionRoute("despesa")]
    public class DespesaComandoController(
        IServiceProvider service,
        IDespesaCommandService _despesaComandoService,
        IParametroDeAlertaDeGastosCommandService _parametroDeAlertaDeGastosCommand
    ) : MainController(service)
    {
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task PostAsync(DespesaCommandDto vendaDto) =>
            await _despesaComandoService.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task PutAsync(Guid code, DespesaCommandDto vendaDto) =>
            await _despesaComandoService.UpdateAsync(code, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(Guid code) => await _despesaComandoService.DeleteAsync(code);

        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task PostRangeAsync(
            IAsyncEnumerable<DespesaCommandDto> vendaDto
        ) => await _despesaComandoService.InsertRangeAsync(vendaDto);

        [HttpPut("parametro-alerta-gastos")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<bool> PutParametroDeAlertaDeGastosAsync(
            List<ParametroDeAlertaDeGastosCommandDto> parametroDeAlertaDeGastosDto
        ) => await _parametroDeAlertaDeGastosCommand.Update(parametroDeAlertaDeGastosDto);
    }
}
