﻿using Application.Queries.Dtos;
using Domain.Dtos;
using Domain.Utilities;

namespace Application.Queries.Interfaces.Telas
{
    public interface IAuditoriaComprasQueryServices
    {
        Task<PagedResult<DespesaDto>> GetListDespesasAllGroups(DespesaFiltroDto despesaFiltroDto, string ano);
        Task<IEnumerable<DespesasSugestaoEconomiaQueryDto>> GetSugestoesEconomiaGraficoAsync();
        Task<IEnumerable<DespesasSugestaoDeFornecedorQueryDto>> SugestaoDeFornecedorMaisBarato(int paginaAtual, int itensPorPagina);
    }
}