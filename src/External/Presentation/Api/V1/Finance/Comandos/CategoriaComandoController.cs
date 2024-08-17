using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Asp.Versioning;
using Domain.Enumeradores;
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
        ICategoriaCommandServices _categoriaComandoServices) : MainController(service)
    {
        #region CRUD
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task PostAsync(CategoriaCommandDto categoriaDto) =>
            await _categoriaComandoServices.InsertAsync(categoriaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task PutAsync(Guid code, CategoriaCommandDto categoriaDto) =>
            await _categoriaComandoServices.UpdateAsync(code, categoriaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(Guid code) => await _categoriaComandoServices.DeleteAsync(code);
        #endregion
    }
}
