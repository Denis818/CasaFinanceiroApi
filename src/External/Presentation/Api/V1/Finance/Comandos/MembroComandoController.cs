using Application.Interfaces.Services.Membros;
using Asp.Versioning;
using Domain.Dtos.Membros;
using Domain.Enumeradores;
using Domain.Models.Membros;
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
    public class MembroComandoController(IServiceProvider service, IMembroAppServices _membroServices) : MainController(service)
    {
        #region CRUD
        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Membro> Post(MembroDto vendaDto) =>
            await _membroServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Membro> Put(int id, MembroDto vendaDto) =>
            await _membroServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> Delete(int id) => await _membroServices.DeleteAsync(id);
        #endregion
    }
}
