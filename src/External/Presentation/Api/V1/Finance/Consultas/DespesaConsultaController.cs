using Application.Queries.Dtos;
using Application.Queries.Interfaces.Telas;
using Asp.Versioning;
using Domain.Dtos;
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
        IAuditoriaComprasQueryServices auditoriaComprasConsultaServices,
        IComparativoFaturasQueryServices comparativoFaturasQueryServices,
        IServiceProvider service
    ) : MainController(service)
    {
        #region Dashboard

        [HttpGet("total-por-grupo")]
        public async Task<IEnumerable<DespesasPorGrupoQueryDto>> GetDespesaGrupoParaGraficoAsync(
            string ano
        ) => await dashboardConsultaServices.GetDespesaGrupoParaGraficoAsync(ano);

        [HttpGet("despesas-dividas-por-membro")]
        public DespesasDivididasMensalQueryDto GetDespesasDivididasMensalAsync() =>
            dashboardConsultaServices.GetDespesasDivididasMensal();

        [HttpGet("relatorio-gastos-grupo")]
        public async Task<RelatorioGastosDoGrupoQueryDto> GetRelatorioDeGastosDoGrupoAsync() =>
            await dashboardConsultaServices.GetRelatorioDeGastosDoGrupoAsync();

        [HttpGet("pdf-despesas-casa")]
        public FileContentResult ExportarPdfRelatorioDeDespesaCasa()
        {
            byte[] pdfBytes = dashboardConsultaServices.ExportarPdfRelatorioDeDespesaCasa();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-casa.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }

        [HttpGet("pdf-despesas-moradia")]
        public FileContentResult ExportarPdfRelatorioDeDespesaMoradia()
        {
            byte[] pdfBytes = dashboardConsultaServices.ExportarPdfRelatorioDeDespesaMoradia();

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
        public async Task<PagedResult<DespesaDto>> GetListDespesasPorGrupo(
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

        [HttpGet("parametro-alerta-gastos")]
        public async Task<object> GetParametroDeAlertaDeGastos() =>
            await painelControleConsultaServices.GetParametroDeAlertaDeGastos();

        #endregion

        #region Auditoria de Compras

        [HttpGet("todos-grupos")]
        public async Task<PagedResult<DespesaDto>> GetListDespesasAllGrupos(
            [FromQuery] DespesaFiltroDto despesaFiltroDto,
            string ano
        )
        {
            return await auditoriaComprasConsultaServices.GetListDespesasAllGroups(
                despesaFiltroDto,
                ano
            );
        }

        [HttpGet("sugestoes-fornecedor")]
        public async Task<
            IEnumerable<DespesasSugestaoDeFornecedorQueryDto>
        > SugestaoDeFornecedorMaisBarato(int paginaAtual = 1, int itensPorPagina = 10)
        {
            return await auditoriaComprasConsultaServices.SugestaoDeFornecedorMaisBarato(
                paginaAtual,
                itensPorPagina
            );
        }

        [HttpGet("sugestoes-economia")]
        public IEnumerable<DespesasSugestaoEconomiaQueryDto> GetSugestoesEconomiaPorGrupoAsync() =>
            auditoriaComprasConsultaServices.GetSugestoesEconomiaGrafico();

        #endregion

        #region Comparativo de Faturas
        [HttpGet("comparativo-faturas")]
        public async Task<ActionResult<List<ComparativoFaturasQueryDto>>> GetComparativoFaturas(
            Guid grupoFaturaCode1,
            Guid grupoFaturaCode2
        )
        {
            var resultado = await comparativoFaturasQueryServices.GetComparativoFaturasAsync(
                grupoFaturaCode1,
                grupoFaturaCode2
            );
            return Ok(resultado);
        }
        #endregion
    }
}
