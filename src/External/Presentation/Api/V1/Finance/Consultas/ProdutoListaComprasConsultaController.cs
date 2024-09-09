using Application.Queries.Dtos;
using Application.Queries.Interfaces.Despesa;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Util;
using Presentation.Version;

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
    }
}
