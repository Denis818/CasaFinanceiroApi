using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Asp.Versioning;
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
    [ApiVersionRoute("despesa")]
    public class DespesaComandoController(
        IServiceProvider service,
        IDespesaCommandService _despesaComandoService,
        IParametroDeAlertaDeGastosCommandService _parametroDeAlertaDeGastosCommand
    ) : MainController(service)
    {
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> PostAsync(DespesaCommandDto vendaDto) =>
            await _despesaComandoService.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Despesa> PutAsync(int id, DespesaCommandDto vendaDto) =>
            await _despesaComandoService.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _despesaComandoService.DeleteAsync(id);

        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRangeAsync(
            IAsyncEnumerable<DespesaCommandDto> vendaDto
        ) => await _despesaComandoService.InsertRangeAsync(vendaDto);

        [HttpPut("parametro-alerta-gastos")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<bool> PutParametroDeAlertaDeGastosAsync(
            List<ParametroDeAlertaDeGastosCommandDto> parametroDeAlertaDeGastosDto
        ) => await _parametroDeAlertaDeGastosCommand.Update(parametroDeAlertaDeGastosDto);
    }
}
