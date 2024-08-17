using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos;
using Domain.Dtos.QueryResults.Despesas;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;

namespace Data.Repository.Despesas
{
    public class DespesaRepository(IServiceProvider service)
        : RepositoryBase<Despesa, FinanceDbContext>(service),
            IDespesaRepository
    {
        public async Task<RelatorioGastosDoGrupoQueryResult> GetRelatorioDeGastosDoGrupoAsync(
            int grupoId,
            CategoriaIdsDto categoriaIds
        )
        {
            var sql =
                @"
                 SELECT 
                     gf.Nome AS GrupoFaturaNome,
                     SUM(CASE 
                             WHEN d.CategoriaId IN ({0}, {1}, {2}) THEN d.Total 
                             ELSE 0 
                         END) AS TotalGastosMoradia,
                     SUM(CASE 
                             WHEN d.CategoriaId NOT IN ({0}, {1}, {2}) THEN d.Total 
                             ELSE 0 
                         END) AS TotalGastosCasa,
                     SUM(d.Total) AS TotalGeral
                 FROM 
                     Despesas d
                 JOIN 
                     Grupo_Fatura gf ON d.GrupoFaturaId = gf.Id
                 WHERE 
                     gf.Id = {3}
                 GROUP BY 
                     gf.Nome";

            var parameters = new object[]
            {
                categoriaIds.CodAluguel,
                categoriaIds.CodCondominio,
                categoriaIds.CodContaDeLuz,
                grupoId
            };

            var result = await ExecuteSqlRawAsync<RelatorioGastosDoGrupoQueryResult>(sql, parameters);

            return result.FirstOrDefault() ?? new RelatorioGastosDoGrupoQueryResult();
        }

        public async Task<IEnumerable<DespesasPorGrupoQueryResult>> GetDespesaGrupoParaGraficoAsync(
            string ano
        )
        {
            var sql =
                @"
                 SELECT 
                     gf.Nome AS GrupoNome,
                     SUM(d.Total) AS Total
                 FROM 
                     Despesas d
                 JOIN 
                     Grupo_Fatura gf ON d.GrupoFaturaId = gf.Id
                 WHERE 
                     gf.Ano = {0}
                 GROUP BY 
                     gf.Nome";

            var parameters = new object[] { ano };

            var despesasPorGrupo = await ExecuteSqlRawAsync<DespesasPorGrupoQueryResult>(
                sql,
                parameters
            );

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

            return despesasPorGrupo
                .OrderBy(dto =>
                {
                    var parts = dto.GrupoNome.Split(' ');
                    var monthName = parts.Length > 2 ? parts[2] : parts[1];
                    return monthOrder[monthName];
                })
                .ToList();
        }

        public async Task<IEnumerable<TotalPorCategoriaQueryResult>> GetTotalPorCategoriaAsync(
            int grupoId
        )
        {
            var sql =
                @"
                 SELECT 
                     c.Descricao AS Categoria,
                     COALESCE(SUM(d.Total), 0) AS Total,
                     COUNT(d.Id) AS QuantidadeDeItens
                 FROM 
                     Despesas d
                 JOIN 
                     Categorias c ON d.CategoriaId = c.Id
                 WHERE 
                     d.GrupoFaturaId = {0}
                 GROUP BY 
                     c.Descricao";

            var parameters = new object[] { grupoId };

            var listAgrupada = await ExecuteSqlRawAsync<TotalPorCategoriaQueryResult>(
                sql,
                parameters
            );

            return listAgrupada;
        }
    }
}
