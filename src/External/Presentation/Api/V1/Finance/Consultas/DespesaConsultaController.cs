using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Asp.Versioning;
using Domain.Dtos.QueryResults.Despesas;
using Domain.Models.Despesas;
using Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Cached;
using Presentation.Attributes.Util;
using Presentation.Version;
using System.Net.Mime;

namespace Presentation.Api.V1.Finance.Consultas
{
    [Cached]
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [ApiVersionRoute("despesa")]
    [GetIdGroupInHeaderFilter]
    public class DespesaConsultaController(
        IDashboardQueryServices dashboardConsultaServices,
        IPainelControleQueryServices painelControleConsultaServices,
        IConferenciaComprasQueryServices conferenciaComprasConsultaServices,
        IServiceProvider service
    ) : MainController(service)
    {
        #region Dashboard

        [HttpGet("total-por-grupo")]
        public async Task<IEnumerable<DespesasPorGrupoResult>> GetDespesaGrupoParaGraficoAsync(string ano) =>
            await dashboardConsultaServices.GetDespesaGrupoParaGraficoAsync(ano);

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoriaQueryDto>> GetTotalPorCategoriaAsync() =>
            await dashboardConsultaServices.GetTotalPorCategoriaAsync();

        [HttpGet("analise-despesa-por-grupo")]
        public async Task<DespesasDivididasMensalQueryDto> GetDespesasDivididasMensalAsync() =>
            await dashboardConsultaServices.GetDespesasDivididasMensalAsync();

        [HttpGet("pdf-despesas-casa")]
        public async Task<FileContentResult> DownloadCalculoCasa()
        {
            byte[] pdfBytes = await dashboardConsultaServices.DownloadPdfRelatorioDeDespesaCasa();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-casa.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }

        [HttpGet("pdf-despesas-moradia")]
        public async Task<FileContentResult> DownloadCalculoMoradia()
        {
            byte[] pdfBytes =
                await dashboardConsultaServices.DownloadPdfRelatorioDeDespesaMoradia();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-Moradia.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }
        #endregion

        #region Painel De controle

        [HttpGet("por-grupo")]
        public async Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
           [FromQuery] DespesaFiltroDto despesaFiltroDto
        )
        {
            return await painelControleConsultaServices.GetListDespesasPorGrupo(despesaFiltroDto);
        }

        [HttpGet("calcular-fatura")]
        public async Task<object> ConferirFaturaDoCartao(double faturaCartao)
        {
            (double totalDespesas, double valorSubtraido) =
                await painelControleConsultaServices.CompararFaturaComTotalDeDespesas(faturaCartao);

            return new { TotalDespesa = totalDespesas, ValorSubtraido = valorSubtraido };
        }

        #endregion

        #region Conferência de Compras

        [HttpGet("todos-grupos")]
        public async Task<PagedResult<Despesa>> GetListDespesasAllGrupos([FromQuery] DespesaFiltroDto despesaFiltroDto, string ano)
        {
            return await conferenciaComprasConsultaServices.GetListDespesasAllGroups(
                despesaFiltroDto, ano
            );
        }

        [HttpGet("sugestoes-fornecedor")]
        public async Task<IEnumerable<DespesasSugestaoDeFornecedorQueryDto>> SugestaoDeFornecedorMaisBarato(
            int paginaAtual = 1,
            int itensPorPagina = 10
        )
        {
            return await conferenciaComprasConsultaServices.SugestaoDeFornecedorMaisBarato(
                paginaAtual,
                itensPorPagina
            );
        }

        [HttpGet("sugestoes-economia")]
        public async Task<
            IEnumerable<DespesasSugestaoEconomiaQueryDto>
        > GetSugestoesEconomiaPorGrupoAsync() =>
            await conferenciaComprasConsultaServices.GetSugestoesEconomiaPorGrupoAsync();

        #endregion
    }
}
