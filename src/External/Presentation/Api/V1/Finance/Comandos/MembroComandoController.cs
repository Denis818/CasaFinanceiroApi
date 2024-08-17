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
    [ApiVersionRoute("membro")]
    public class MembroComandoController(
        IServiceProvider service,
        IMembroComandoServices _membroComandoServices
    ) : MainController(service)
    {
        #region CRUD
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Post(MembroCommandDto vendaDto) =>
            await _membroComandoServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task Put(Guid code, MembroCommandDto vendaDto) =>
            await _membroComandoServices.UpdateAsync(code, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> Delete(Guid code) => await _membroComandoServices.DeleteAsync(code);
        #endregion
    }
}
