using Application.Configurations.MappingsApp;
using Application.Helpers;
using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Application.Queries.Interfaces.Despesa;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Application.Utilities;
using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Domain.Utilities;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services.Telas
{
    public class PainelControleQueryServices(
        IServiceProvider service,
        IProdutoListaComprasQueryService _ProdutoListaComprasQueryService,
        IParametroDeAlertaDeGastosRepository _parametroDeAlertaDeGastosRepository
    )
        : BaseQueryService<Despesa, DespesaQueryDto, IDespesaRepository>(service),
            IPainelControleQueryServices
    {
        protected override DespesaQueryDto MapToDTO(Despesa entity) => entity.MapToDTO();

        #region Painel de Controle

        public async Task<PagedResult<DespesaQueryDto>> GetListDespesasPorGrupo(
            DespesaFiltroDto despesaFiltroDto
        )
        {
            if (string.IsNullOrEmpty(despesaFiltroDto.Filter))
            {
                return await GetAllDespesas(
                    _queryDespesasPorGrupo,
                    despesaFiltroDto.PaginaAtual,
                    despesaFiltroDto.ItensPorPagina
                );
            }

            var query = GetDespesasFiltradas(
                _queryDespesasPorGrupo,
                despesaFiltroDto.Filter,
                despesaFiltroDto.TipoFiltro
            );

            var listaPaginada = await Pagination.PaginateResultAsync(
                query.Select(d => d.MapToDTO()),
                despesaFiltroDto.PaginaAtual,
                despesaFiltroDto.ItensPorPagina
            );

            return listaPaginada;
        }

        public async Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao)
        {
            double totalDespesas = await _queryDespesasPorGrupo
                .Where(despesa =>
                    despesa.Categoria.Code != _categoriaIds.CodAluguel
                    && despesa.Categoria.Code != _categoriaIds.CodContaDeLuz
                    && despesa.Categoria.Code != _categoriaIds.CodCondominio
                )
                .SumAsync(despesa => despesa.Total);

            double valorSubtraido = totalDespesas - faturaCartao;

            return (totalDespesas, valorSubtraido);
        }

        public async Task<IList<ParametroDeAlertaDeGastos>> GetParametroDeAlertaDeGastos()
        {
            return await _parametroDeAlertaDeGastosRepository.Get().ToListAsync();
        }

        #endregion

        #region Filter Despesas

        private IOrderedQueryable<Despesa> GetDespesasFiltradas(
            IQueryable<Despesa> query,
            string filter,
            EnumFiltroDespesa tipoFiltro
        )
        {
            switch (tipoFiltro)
            {
                case EnumFiltroDespesa.Item:
                query = query.Where(despesa =>
                    despesa.Item.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.Categoria:
                query = query.Where(despesa =>
                    despesa.Categoria.Descricao.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.Fornecedor:
                query = query.Where(despesa =>
                    despesa.Fornecedor.ToLower().Contains(filter.ToLower())
                );
                break;
            }

            return query.OrderByDescending(d => d.DataCompra);
        }

        private async Task<PagedResult<DespesaQueryDto>> GetAllDespesas(
            IQueryable<Despesa> query,
            int paginaAtual,
            int itensPorPagina
        )
        {
            var queryAll = query
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura)
                .OrderByDescending(d => d.DataCompra)
                .Select(d => d.MapToDTO());

            var despesas = await Pagination.PaginateResultAsync(
                queryAll,
                paginaAtual,
                itensPorPagina
            );

            if (despesas.TotalItens == 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
            }

            return despesas;
        }

        #endregion

        #region Exporta Lista de Compras

        public async Task<byte[]> ExportaPdfListaDeComprasAsync()
        {
            var customStyle = new PdfTableStyle(widthPercentage: 50);

            var pdfHelper = new PdfTableHelper(customStyle);

            var ProdutoListaCompras = await _ProdutoListaComprasQueryService.GetAllAsync() ?? [];

            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdfDocument = new PdfDocument(writer);
            using var doc = new Document(pdfDocument);

            pdfHelper.CreateTitleDocument(doc, "Lista de Compras");

            var itensProdutoListaCompras = ProdutoListaCompras.Select(x => x.Item).ToList();

            pdfHelper.CreateSingleColumnTable(doc, itensProdutoListaCompras);

            doc.Close();
            return memoryStream.ToArray();
        }

        #endregion
    }
}
