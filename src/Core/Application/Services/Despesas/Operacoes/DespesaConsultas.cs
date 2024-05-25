﻿using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Despesas.Base;
using Application.Utilities;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Despesas.Operacoes
{
    public class DespesaConsultas(
        IServiceProvider service,
        IGrupoDespesaRepository _grupoDespesaRepository,
        IDespesaMoradiaAppService _despesaMoradiaApp,
        IDespesaCasaAppService _despesaCasaApp
    ) : BaseDespesaService(service), IDespesaConsultas
    {
        public async Task<IEnumerable<DespesasPorFornecedorDto>> MediaDespesasPorFornecedorAsync(
            int paginaAtual,
            int itensPorPagina
        )
        {
            var categorias = await _categoriaRepository.Get().ToListAsync();
            List<DespesasPorFornecedorDto> sugestoes = new();

            foreach (var categoria in categorias)
            {
                var despesasTotalPorCategoria = await GetDespesasPorCategoriaAsync(categoria.Id);

                var mediasPorFornecedor = despesasTotalPorCategoria
                    .GroupBy(d => d.Fornecedor.ToLower())
                    .Select(group => new
                    {
                        Fornecedor = group.Key,
                        MediaPreco = group.Average(d => d.Preco),
                    })
                    .OrderBy(x => x.MediaPreco)
                    .ToList();

                foreach (var mediaPorFornecedor in mediasPorFornecedor)
                {
                    var itensDesteFornecedor = _queryDespesasPorGrupo.Where(despesa =>
                        despesa.Fornecedor.ToLower() == mediaPorFornecedor.Fornecedor.ToLower()
                    );

                    sugestoes.Add(
                        new DespesasPorFornecedorDto
                        {
                            MediaDeFornecedor =
                                $"{categoria.Descricao} em {mediaPorFornecedor.Fornecedor}, teve um gasto médio de R$ {mediaPorFornecedor.MediaPreco.ToFormatPrBr()}.",

                            ItensDesteFornecedor = await Pagination.PaginateResultAsync(
                                itensDesteFornecedor,
                                paginaAtual,
                                itensPorPagina
                            )
                        }
                    );
                }
            }

            if (sugestoes.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    "Nenhuma sugestão de otimização disponível no momento."
                );
            }

            return sugestoes;
        }

        #region Listagem das Despesas

        public async Task<Despesa> GetByIdAsync(int id)
        {
            var despesa = await _repository
                .Get(despesa => despesa.Id == id)
                .Include(x => x.Categoria)
                .Include(x => x.GrupoDespesa)
                .FirstOrDefaultAsync();

            return despesa;
        }

        public async Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
            string filter,
            int paginaAtual,
            int itensPorPagina,
            EnumFiltroDespesa tipoFiltro
        )
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await GetAllDespesas(_queryDespesasPorGrupo, paginaAtual, itensPorPagina);
            }

            IOrderedQueryable<Despesa> query = GetDespesasFiltradas(
                _queryDespesasPorGrupo,
                filter,
                tipoFiltro
            );

            var listaPaginada = await Pagination.PaginateResultAsync(
                query,
                paginaAtual,
                itensPorPagina
            );

            return listaPaginada;
        }

        public async Task<PagedResult<Despesa>> GetListDespesasAllGroups(
            string filter,
            int paginaAtual,
            int itensPorPagina,
            EnumFiltroDespesa tipoFiltro
        )
        {
            var queryDespesasAllGrupo = _repository
                .Get()
                .Include(c => c.Categoria)
                .Include(c => c.GrupoDespesa);

            if (string.IsNullOrEmpty(filter))
            {
                return await GetAllDespesas(queryDespesasAllGrupo, paginaAtual, itensPorPagina);
            }

            IOrderedQueryable<Despesa> query = GetDespesasFiltradas(
                queryDespesasAllGrupo,
                filter,
                tipoFiltro
            );

            var listaPaginada = await Pagination.PaginateResultAsync(
                query,
                paginaAtual,
                itensPorPagina
            );

            return listaPaginada;
        }

        #endregion

        #region Análise de Depesas
        public async Task<IEnumerable<DespesasTotalPorCategoriaDto>> GetTotalPorCategoriaAsync()
        {
            var listDespesas = await _queryDespesasPorGrupo.ToListAsync();

            if (listDespesas.Count <= 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
                return [];
            }

            var listAgrupada = listDespesas.GroupBy(despesa => despesa.Categoria.Descricao);

            return listAgrupada.Select(list => new DespesasTotalPorCategoriaDto(
                list.Key,
                list.Sum(despesa => despesa.Total)
            ));
        }

        public async Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync()
        {
            // Mapeamento de nomes de meses para valores numéricos
            var monthOrder = new Dictionary<string, int>
            {
                { "Janeiro", 1 },
                { "Fevereiro", 2 },
                { "Março", 3 },
                { "Abril", 4 },
                { "Maio", 5 },
                { "Junho", 6 },
                { "Julho", 7 },
                { "Agosto", 8 },
                { "Setembro", 9 },
                { "Outubro", 10 },
                { "Novembro", 11 },
                { "Dezembro", 12 }
            };

            var despesasPorGrupo = _repository
                .Get()
                .GroupBy(d => d.GrupoDespesa.Nome)
                .Select(group => new DespesasPorGrupoDto
                {
                    GrupoNome = group.Key,
                    Total = group.Sum(d => d.Total)
                })
                .AsEnumerable() // Processamento local para manipulação de strings e datas
                .OrderBy(dto =>
                {
                    // Extrai o nome do mês do GrupoNome
                    var monthName = dto.GrupoNome.Split(' ')[2]; // Assumindo "Fatura de Janeiro 2024"
                    // Usa o dicionário para encontrar a ordem do mês
                    return monthOrder[monthName];
                });

            return await Task.FromResult(despesasPorGrupo.ToList());
        }

        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync()
        {
            //Aluguel + Condomínio + Conta de Luz
            var distribuicaoCustosMoradia =
                await _despesaMoradiaApp.CalcularDistribuicaoCustosMoradiaAsync();

            //Despesas de casa como almoço, Limpeza, higiene etc...
            var distribuicaoCustosCasa =
                await _despesaCasaApp.CalcularDistribuicaoCustosCasaAsync();

            var despesasPorMembro = await DistribuirDespesasEntreMembrosAsync(
                distribuicaoCustosCasa.DespesaGeraisMaisAlmocoDividioPorMembro,
                distribuicaoCustosCasa.TotalAlmocoParteDoJhon,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaDoPeu
            );

            var relatorioGastosDoGrupo = await GetRelatorioDeGastosDoMesAsync();

            return new DespesasDivididasMensalDto
            {
                RelatorioGastosDoGrupo = relatorioGastosDoGrupo,

                DespesasPorMembro = despesasPorMembro
            };
        }
        #endregion

        #region Metodos de Suporte
        private IOrderedQueryable<Despesa> GetDespesasFiltradas(
            IQueryable<Despesa> query,
            string filter,
            EnumFiltroDespesa tipoFiltro
        )
        {
            switch (tipoFiltro)
            {
                case EnumFiltroDespesa.Item:
                    query = query.Where(despesa =>
                        despesa.Item.ToLower().Contains(filter.ToLower())
                    );
                    break;

                case EnumFiltroDespesa.Categoria:
                    query = query.Where(despesa =>
                        despesa.Categoria.Descricao.ToLower().Contains(filter.ToLower())
                    );
                    break;

                case EnumFiltroDespesa.Fornecedor:
                    query = query.Where(despesa =>
                        despesa.Fornecedor.ToLower().Contains(filter.ToLower())
                    );
                    break;
            }

            return query.OrderByDescending(d => d.DataCompra);
        }

        private async Task<PagedResult<Despesa>> GetAllDespesas(
            IQueryable<Despesa> query,
            int paginaAtual,
            int itensPorPagina
        )
        {
            var queryAll = query
                .Include(c => c.Categoria)
                .Include(c => c.GrupoDespesa)
                .OrderByDescending(d => d.DataCompra);

            var despesas = await Pagination.PaginateResultAsync(
                queryAll,
                paginaAtual,
                itensPorPagina
            );

            if (despesas.TotalItens == 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
            }

            return despesas;
        }

        private async Task<List<Despesa>> GetDespesasPorCategoriaAsync(int idCategoria)
        {
            if (
                idCategoria == _categoriaIds.IdAluguel
                || idCategoria == _categoriaIds.IdCondominio
                || idCategoria == _categoriaIds.IdContaDeLuz
                || idCategoria == _categoriaIds.IdInternet
            )
            {
                return [];
            }

            return await _queryDespesasPorGrupo
                .Where(d => d.CategoriaId == idCategoria)
                .Include(c => c.Categoria)
                .Include(g => g.GrupoDespesa)
                .OrderByDescending(d => d.DataCompra)
                .ToListAsync();
        }

        private async Task<RelatorioGastosDoGrupoDto> GetRelatorioDeGastosDoMesAsync()
        {
            string grupoNome = _grupoDespesaRepository
                .Get(g => g.Id == _grupoDespesaId)
                .FirstOrDefault()
                ?.Nome;

            if (grupoNome.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.SelecioneUmGrupoDesesa);
                return new();
            }

            double totalGastoMoradia = await _queryDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id == _categoriaIds.IdAluguel
                    || d.Categoria.Id == _categoriaIds.IdCondominio
                    || d.Categoria.Id == _categoriaIds.IdContaDeLuz
                )
                .SumAsync(d => d.Total);

            double totalGastosCasa = await _queryDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id != _categoriaIds.IdAluguel
                    && d.Categoria.Id != _categoriaIds.IdCondominio
                    && d.Categoria.Id != _categoriaIds.IdContaDeLuz
                )
                .SumAsync(d => d.Total);

            var totalGeral = totalGastoMoradia + totalGastosCasa;

            return new RelatorioGastosDoGrupoDto
            {
                GrupoDespesaNome = grupoNome,
                TotalGeral = totalGeral.RoundTo(2),
                TotalGastosCasa = totalGastosCasa.RoundTo(2),
                TotalGastosMoradia = totalGastoMoradia.RoundTo(2),
            };
        }

        private async Task<IEnumerable<DespesaPorMembroDto>> DistribuirDespesasEntreMembrosAsync(
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double almocoParteDoJhon,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu
        )
        {
            var todosMembros = await _membroRepository.Get().ToListAsync();

            double ValorMoradia(Membro membro)
            {
                if (membro.Id == _membroId.IdPeu)
                {
                    return aluguelCondominioContaLuzParaPeu.RoundTo(2);
                }
                else
                {
                    return aluguelCondominioContaLuzPorMembroForaPeu.RoundTo(2);
                }
            }

            var valoresPorMembro = todosMembros.Select(member => new DespesaPorMembroDto
            {
                Nome = member.Nome,

                ValorDespesaCasa =
                    member.Id == _membroId.IdJhon
                        ? Math.Max(almocoParteDoJhon.RoundTo(2), 0)
                        : Math.Max(despesaGeraisMaisAlmocoDividioPorMembro.RoundTo(2), 0),

                ValorDespesaMoradia =
                    member.Id == _membroId.IdJhon ? -1 : ValorMoradia(member).RoundTo(2)
            });

            return valoresPorMembro;
        }
        #endregion
    }
}
