using Application.Interfaces.Services.Categorias;
using Asp.Versioning;
using Domain.Dtos.Categorias;
using Domain.Enumeradores;
using Domain.Models.Categorias;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Cached;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.Comandos
{
    [ClearCache]
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [ApiVersionRoute("categoria")]
    public class CategoriaComandoController(
        IServiceProvider service,
        ICategoriaAppServices _categoriaServices) : MainController(service)
    {
        #region CRUD
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> PostAsync(CategoriaDto categoriaDto) =>
            await _categoriaServices.InsertAsync(categoriaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Categoria> PutAsync(int id, CategoriaDto categoriaDto) =>
            await _categoriaServices.UpdateAsync(id, categoriaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _categoriaServices.DeleteAsync(id);
        #endregion
    }
}
