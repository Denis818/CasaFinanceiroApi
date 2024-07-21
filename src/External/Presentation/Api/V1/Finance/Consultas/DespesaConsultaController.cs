using System.Net.Mime;
using Application.Interfaces.Services.Finance.Consultas;
using Asp.Versioning;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Filtro;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Cached;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.Consultas
{
    [Cached]
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [ApiVersionRoute("despesa")]
    [GetIdGroupInHeaderFilter]
    public class DespesaConsultaController(
        IDashboardConsultaServices dashboardConsultaServices,
        IPainelControleConsultaServices painelControleConsultaServices,
        IConferenciaVendasConsultaServices conferenciaVendasConsultaServices,
        IServiceProvider service
    ) : MainController(service)
    {
        #region Dashboard

        [HttpGet("total-por-grupo")]
        public async Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync() =>
            await dashboardConsultaServices.GetDespesaGrupoParaGraficoAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoriaDto>> GetTotalPorCategoriaAsync() =>
            await dashboardConsultaServices.GetTotalPorCategoriaAsync();

        [HttpGet("analise-despesa-por-grupo")]
        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync() =>
            await dashboardConsultaServices.GetAnaliseDesesasPorGrupoAsync();

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

        #region Conferência de compras

        [HttpGet("todos-grupos")]
        public async Task<PagedResult<Despesa>> GetListDespesasAllGrupos(
         [FromQuery] DespesaFiltroDto despesaFiltroDto
        )
        {
            return await conferenciaVendasConsultaServices.GetListDespesasAllGroups(
                despesaFiltroDto
            );
        }

        [HttpGet("sugestoes-fornecedor")]
        public async Task<IEnumerable<SugestaoDeFornecedorDto>> SugestaoDeFornecedorMaisBarato(
            int paginaAtual = 1,
            int itensPorPagina = 10
        )
        {
            return await conferenciaVendasConsultaServices.SugestaoDeFornecedorMaisBarato(
                paginaAtual,
                itensPorPagina
            );
        }

        [HttpGet("sugestoes-economia")]
        public async Task<
            IEnumerable<SugestaoEconomiaInfoDto>
        > GetSugestoesEconomiaPorGrupoAsync() =>
            await conferenciaVendasConsultaServices.GetSugestoesEconomiaPorGrupoAsync();

        #endregion
    }
}
