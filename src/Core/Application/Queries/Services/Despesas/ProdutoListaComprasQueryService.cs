using Application.Configurations.MappingsApp;
using Application.Helpers;
using Application.Queries.Dtos;
using Application.Queries.Interfaces.Despesa;
using Application.Queries.Services.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services
{
    public class ProdutoListaComprasQueryService(IServiceProvider service)
        : BaseQueryService<ProdutoListaCompras, ProdutoListaComprasQueryDto, IProdutoListaComprasRepository>(service), IProdutoListaComprasQueryService
    {
        protected override ProdutoListaComprasQueryDto MapToDTO(ProdutoListaCompras entity) => entity.MapToDTO();

        public async Task<IEnumerable<ProdutoListaComprasQueryDto>> GetAllAsync() =>
            await _repository
                .Get()
                .OrderBy(c => c.Item)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .ToListAsync();

        public async Task<byte[]> ExportaPdfListaDeComprasAsync()
        {
            var customStyle = new PdfTableStyle(widthPercentage: 50);

            var pdfHelper = new PdfTableHelper(customStyle);

            var ProdutoListaCompras = await GetAllAsync() ?? [];

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
    }
}
