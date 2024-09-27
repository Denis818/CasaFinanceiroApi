using Application.Queries.Dtos;
using Application.Queries.Interfaces.ListaCompras;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Util;
using Presentation.Version;
using System.Net.Mime;

namespace Presentation.Api.V1.Finance.Consultas
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [ApiVersionRoute("lista-compras")]
    public class ProdutoListaComprasConsultaController(
        IServiceProvider service,
        IProdutoListaComprasQueryService _ProdutoListaComprasQueryService
    ) : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<ProdutoListaComprasQueryDto>> GetAllAsync() =>
            await _ProdutoListaComprasQueryService.GetAllAsync();

        [HttpGet("pdf-lista-compras")]
        public async Task<FileContentResult> ExportaPdfListaDeComprasAsync()
        {
            byte[] pdfBytes =
                await _ProdutoListaComprasQueryService.ExportaPdfListaDeComprasAsync();

            var contentDisposition = new ContentDisposition
            {
                FileName = "lista-de-compras.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }
    }
}
