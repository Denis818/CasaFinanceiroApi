using Application.Queries.Interfaces.Despesa;
using Asp.Versioning;
using Domain.Dtos.Despesas;
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
    public class ListaComprasConsultaController(
        IServiceProvider service,
        IListaComprasQueryService _listaComprasQueryService
    ) : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<ListaComprasQueryDto>> GetAllAsync() =>
            await _listaComprasQueryService.GetAllAsync();
    }
}
