using Application.Commands.Dtos.Despesa;
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
    [ApiVersionRoute("lista-compras")]
    public class ListaComprasComandoController(IServiceProvider service, IListaComprasCommandService listaComprasCommandService)
        : MainController(service)
    {
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<bool> PostAsync(ListaComprasCommandDto item) =>
              await listaComprasCommandService.InsertAsync(item);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<bool> PutAsync(Guid code, ListaComprasCommandDto item) =>
            await listaComprasCommandService.UpdateAsync(code, item);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(Guid code) => await listaComprasCommandService.DeleteAsync(code);

    }
}
