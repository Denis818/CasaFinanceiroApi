using Application.Interfaces.Services.Finance.Comandos;
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
    [ApiVersionRoute("despesa")]
    public class DespesaComandoController(
        IServiceProvider service,
        IDespesaComandoService _despesaComandoService
    ) : MainController(service)
    {
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> PostAsync(DespesaDto vendaDto) =>
            await _despesaComandoService.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Despesa> PutAsync(int id, DespesaDto vendaDto) =>
            await _despesaComandoService.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _despesaComandoService.DeleteAsync(id);

        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRangeAsync(
            IAsyncEnumerable<DespesaDto> vendaDto
        ) => await _despesaComandoService.InsertRangeAsync(vendaDto);
    }
}
