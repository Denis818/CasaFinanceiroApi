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
    public class ProdutoListaComprasComandoController(IServiceProvider service, IProdutoListaComprasCommandService ProdutoListaComprasCommandService)
        : MainController(service)
    {
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.LISTACOMPRA_000001)]
        public async Task<bool> PostAsync(ProdutoListaComprasCommandDto item) =>
              await ProdutoListaComprasCommandService.InsertAsync(item);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.LISTACOMPRA_000002)]
        public async Task<bool> PutAsync(Guid code, ProdutoListaComprasCommandDto item) =>
            await ProdutoListaComprasCommandService.UpdateAsync(code, item);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.LISTACOMPRA_000003)]
        public async Task<bool> DeleteAsync(Guid code) => await ProdutoListaComprasCommandService.DeleteAsync(code);

    }
}
