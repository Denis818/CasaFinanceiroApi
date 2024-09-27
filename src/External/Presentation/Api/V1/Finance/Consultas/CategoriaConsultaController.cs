using Application.Queries.Interfaces;
using Asp.Versioning;
using Domain.Dtos.Categoria;
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
    [ApiVersionRoute("categoria")]
    public class CategoriaConsultaController(
        IServiceProvider service,
        ICategoriaQueryServices _categoriaConsultaServices
    ) : MainController(service)
    {
        [HttpGet]
        [GetIdGroupInHeaderFilter]
        public async Task<IEnumerable<CategoriaDto>> GetAllAsync() =>
            await _categoriaConsultaServices.GetAllAsync();
    }
}
