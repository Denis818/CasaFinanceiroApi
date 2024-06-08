﻿using Application.Interfaces.Services.Despesas;
using Application.Services;
using Asp.Versioning;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.GrupoFaturas
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [Route("api/v1/grupo-fatura")]
    public class GrupoFaturaController(
        IServiceProvider service,
        IGrupoFaturaAppService _grupoFaturaServices,
        IStatusFaturaAppService _statusFaturaServices
    ) : MainController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<IEnumerable<GrupoFatura>> GetAllAsync() =>
            await _grupoFaturaServices.GetAllAsync();

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<GrupoFatura> PostAsync(GrupoFaturaDto grupoFaturaDto) =>
            await _grupoFaturaServices.InsertAsync(grupoFaturaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<GrupoFatura> PutAsync(int id, GrupoFaturaDto grupoFaturaDto) =>
            await _grupoFaturaServices.UpdateAsync(id, grupoFaturaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _grupoFaturaServices.DeleteAsync(id);
        #endregion

        #region Status Fatura

        [HttpGet("status-fatura")]
        public async Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status) =>
           await _statusFaturaServices.GetStatusFaturaDtoByNameAsync(status);

        [HttpPut("status-fatura")]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<StatusFatura> PutStatusFaturaAsync(EnumFaturaNome faturaNome, EnumStatusFatura status) =>
            await _statusFaturaServices.UpdateAsync(faturaNome, status);

        #endregion
    }
}
