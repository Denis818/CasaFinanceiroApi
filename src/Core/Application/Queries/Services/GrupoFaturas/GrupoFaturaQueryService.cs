﻿using Application.Configurations.MappingsApp;
using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Domain.Converters.DatesTimes;
using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.GrupoFaturas;
using Domain.Models.GrupoFaturas;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Queries.Services
{
    public class GrupoFaturaQueryService(
        IServiceProvider service,
        IStatusFaturaRepository _statusFaturaRepository)
        : BaseQueryService<GrupoFatura, GrupoFaturaDto, IGrupoFaturaRepository>(service),
            IGrupoFaturaQueryService
    {
        protected override GrupoFaturaDto MapToDTO(GrupoFatura entity) => entity.MapToDTO();

        public async Task<IEnumerable<GrupoFaturaDto>> GetAllAsync(string ano)
        {
            var listGruposFaturas = await _repository
                .Get(fatura => fatura.Ano == ano)
                .OrderByDescending(fatura => fatura.DataCriacao.Date)
                .Include(s => s.StatusFaturas)
                .Include(fatura => fatura.Despesas)
                .Select(fatura => new GrupoFaturaDto
                {
                    Code = fatura.Code,
                    Nome = fatura.Nome,
                    Ano = fatura.Ano,
                    Desconto = fatura.Desconto,
                    QuantidadeDespesas = fatura.Despesas.Count,
                    TotalDespesas = fatura.Despesas.Sum(despesa => despesa.Total),
                    StatusFaturas = fatura
                        .StatusFaturas.Select(s => new StatusFaturaDto
                        {
                            Code = s.Code,
                            FaturaNome = s.FaturaNome,
                            Estado = s.Estado
                        })
                        .ToList(),
                })
                .ToListAsync();

            return listGruposFaturas;
        }

        public async Task<
            IEnumerable<GrupoFaturaSeletorQueryDto>
        > GetListGrupoFaturaParaSeletorAsync(string ano)
        {
            var listGruposFaturas = await BuscarGrupoFaturaParaSeletorAsync(ano);

            if (listGruposFaturas.Count == 0)
            {
                await CreateDefaultGroupFatureAsync(ano);
                listGruposFaturas = await BuscarGrupoFaturaParaSeletorAsync(ano);
            }

            return listGruposFaturas;
        }

        public async Task<string> GetNameFatura(Guid code)
        {
            var fatura = await _repository.Get(fatura => fatura.Code == code).FirstOrDefaultAsync();

            return fatura == null ? "Não encontrado" : fatura.Nome;
        }

        public async Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status)
        {
            var grupoFaturaCode = (Guid)_httpContext.Items["grupo-fatura-code"];

            var statusFatura = await _statusFaturaRepository
                .Get(s => s.GrupoFatura.Code == grupoFaturaCode)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Estado == status);

            if (statusFatura == null)
            {
                var defaultState = status.Contains("Casa")
                    ? EnumStatusFatura.CasaFechado.ToString()
                    : EnumStatusFatura.MoradiaFechado.ToString();

                return new StatusFaturaDto { Estado = defaultState };
            }

            return new StatusFaturaDto { Estado = statusFatura.Estado };
        }

        #region Metodos de Suporte

        private async Task<IList<GrupoFaturaSeletorQueryDto>> BuscarGrupoFaturaParaSeletorAsync(
            string ano
        )
        {
            var listGruposFaturas = await _repository
                .Get(fatura => fatura.Ano == ano)
                .OrderByDescending(fatura => fatura.DataCriacao.Date)
                .AsNoTracking()
                .Select(fatura => new GrupoFaturaSeletorQueryDto
                {
                    Nome = fatura.Nome,
                    Code = fatura.Code
                })
                .ToListAsync();

            return listGruposFaturas;
        }

        private async Task CreateDefaultGroupFatureAsync(string ano)
        {
            var dataCriacao = DateTimeZoneConverterPtBR.GetBrasiliaDateTimeZone();

            string mesAtualName = dataCriacao.ToString("MMMM", new CultureInfo("pt-BR"));

            mesAtualName = char.ToUpper(mesAtualName[0]) + mesAtualName[1..].ToLower();

            var grupoFatura = new GrupoFatura
            {
                Nome = $"Fatura de {mesAtualName} {ano}",
                Ano = ano,
                DataCriacao = dataCriacao,
                StatusFaturas =
                [
                    new()
                    {
                        Estado = EnumStatusFatura.CasaAberto.ToString(),
                        FaturaNome = EnumFaturaTipo.Casa.ToString()
                    },
                    new()
                    {
                        Estado = EnumStatusFatura.MoradiaAberto.ToString(),
                        FaturaNome = EnumFaturaTipo.Moradia.ToString()
                    }
                ]
            };

            await _grupoFaturaRepository.InsertAsync(grupoFatura);
            await _grupoFaturaRepository.SaveChangesAsync();
        }
        #endregion
    }
}
