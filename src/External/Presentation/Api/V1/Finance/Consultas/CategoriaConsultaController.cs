using Application.Interfaces.Services.Finance.Consultas;
using Asp.Versioning;
using Domain.Models.Categorias;
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
        ICategoriaConsultaServices _categoriaConsultaServices
    ) : MainController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _categoriaConsultaServices.GetAllAsync();
    }
}
