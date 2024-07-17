using Application.Interfaces.Services.Despesas;
using Asp.Versioning;
using Domain.Dtos.Despesas.Consultas;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Cached;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.Despesas
{
    [Cached]
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [Route("api/v1/despesa")]
    [GetIdGroupInHeaderFilter]
    public class DespesaConsultaController(
        IServiceProvider service,
        IDespesaConsultas _despesaConsultas
    ) : MainController(service)
    {
        #region Dashboard

        [HttpGet("total-por-grupo")]
        public async Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync() =>
            await _despesaConsultas.GetDespesaGrupoParaGraficoAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoriaDto>> GetTotalPorCategoriaAsync() =>
            await _despesaConsultas.GetTotalPorCategoriaAsync();

        [HttpGet("analise-despesa-por-grupo")]
        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync() =>
            await _despesaConsultas.GetAnaliseDesesasPorGrupoAsync();

        #endregion

        #region Painel De controle

        [HttpGet("por-grupo")]
        public async Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
            string filter,
            int paginaAtual = 1,
            int itensPorPagina = 10,
            EnumFiltroDespesa tipoFiltro = EnumFiltroDespesa.Item
        )
        {
            return await _despesaConsultas.GetListDespesasPorGrupo(
                filter,
                paginaAtual,
                itensPorPagina,
                tipoFiltro
            );
        }

        #endregion

        #region Conferência de compras

        [HttpGet("todos-grupos")]
        public async Task<PagedResult<Despesa>> GetListDespesasAllGrupos(string filter, int paginaAtual = 1,
            int itensPorPagina = 10, EnumFiltroDespesa tipoFiltro = EnumFiltroDespesa.Item) =>
            await _despesaConsultas.GetListDespesasAllGroups(filter, paginaAtual, itensPorPagina, tipoFiltro);

        [HttpGet("sugestoes-fornecedor")]
        public async Task<IEnumerable<SugestaoDeFornecedorDto>> SugestaoDeFornecedorMaisBarato(
            int paginaAtual = 1,
            int itensPorPagina = 10) => await _despesaConsultas.SugestaoDeFornecedorMaisBarato(paginaAtual, itensPorPagina);

        [HttpGet("sugestoes-economia")]
        public async Task<IEnumerable<SugestaoEconomiaInfoDto>> GetSugestoesEconomiaPorGrupoAsync() =>
            await _despesaConsultas.GetSugestoesEconomiaPorGrupoAsync();

        #endregion
    }
}
