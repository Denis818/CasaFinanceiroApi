using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Domain.Converters.DatesTimes;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Queries.Services
{
    public class GrupoFaturaQueryService(
        IServiceProvider service,
        IStatusFaturaRepository _statusFaturaRepository,
        IGrupoFaturaRepository _grupoFaturaRepository
    ) : BaseQueryService<GrupoFatura, IGrupoFaturaRepository>(service), IGrupoFaturaQueryService
    {
        public async Task<IEnumerable<GrupoFaturaQueryDto>> GetAllAsync(string ano)
        {
            var listGruposFaturas = await _repository
                .Get(fatura => fatura.Ano == ano)
                .Include(s => s.StatusFaturas)
                .Include(fatura => fatura.Despesas)
                .Select(fatura => new GrupoFaturaQueryDto
                {
                    Id = fatura.Id,
                    Nome = fatura.Nome,
                    Ano = fatura.Ano,
                    StatusFaturas = fatura.StatusFaturas,
                    QuantidadeDespesas = fatura.Despesas.Count,
                    TotalDespesas = fatura.Despesas.Sum(despesa => despesa.Total),
                })
                .OrderBy(c => c.Nome)
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

        public async Task<string> GetNameFatura(int id)
        {
            var fatura = await _repository.Get(fatura => fatura.Id == id).FirstOrDefaultAsync();

            return fatura == null ? "Não encontrado" : fatura.Nome;
        }

        public async Task<StatusFaturaQueryDto> GetStatusFaturaDtoByNameAsync(string status)
        {
            var grupoFaturaId = (int)(_httpContext.Items["GrupoFaturaId"] ?? 0);

            var statusFatura = await _statusFaturaRepository
                .Get(s => s.GrupoFaturaId == grupoFaturaId)
                .FirstOrDefaultAsync(s => s.Estado == status);

            if (statusFatura == null)
            {
                var defaultState = status.Contains("Casa")
                    ? EnumStatusFatura.CasaFechado.ToString()
                    : EnumStatusFatura.MoradiaFechado.ToString();

                return new StatusFaturaQueryDto { Estado = defaultState };
            }

            return new StatusFaturaQueryDto { Estado = statusFatura.Estado };
        }

        #region Metodos de Suporte


        private async Task<IList<GrupoFaturaSeletorQueryDto>> BuscarGrupoFaturaParaSeletorAsync(
            string ano
        )
        {
            var listGruposFaturas = await _repository
                .Get(fatura => fatura.Ano == ano)
                .Select(fatura => new GrupoFaturaSeletorQueryDto
                {
                    Nome = fatura.Nome,
                    Id = fatura.Id
                })
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return listGruposFaturas;
        }

        private async Task CreateDefaultGroupFatureAsync(string ano)
        {
            var dataCriacao = DateTimeZoneProvider.GetBrasiliaDateTimeZone();

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
