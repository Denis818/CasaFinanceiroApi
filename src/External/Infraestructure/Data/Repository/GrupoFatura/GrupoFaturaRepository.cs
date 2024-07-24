using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos.QueryResults;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Repository
{
    public class GrupoFaturaRepository(IServiceProvider service)
        : RepositoryBase<GrupoFatura, FinanceDbContext>(service),
            IGrupoFaturaRepository
    {
        public async Task<GrupoFatura> ExisteAsync(int id, string nome)
        {
            GrupoFatura GrupoFatura = null;
            if (nome != null)
            {
                GrupoFatura = await Get(c => c.Nome == nome).FirstOrDefaultAsync();
            }
            else
            {
                GrupoFatura = await Get(c => c.Id == id).FirstOrDefaultAsync();
            }

            return GrupoFatura;
        }

        public async Task<IEnumerable<GrupoFaturaQueryResult>> GetAllAsync(string ano)
        {
            var sql =
                @"
                 SELECT 
                     gf.Id,
                     gf.Nome,
                     gf.Ano,
                     COUNT(d.Id) AS QuantidadeDespesas,
                     COALESCE(SUM(d.Total), 0) AS TotalDespesas
                 FROM 
                     Grupo_Fatura gf
                 LEFT JOIN 
                     Despesas d ON gf.Id = d.GrupoFaturaId
                 LEFT JOIN 
                     Status_Faturas sf ON gf.Id = sf.GrupoFaturaId
                 WHERE 
                     gf.Ano = {0}
                 GROUP BY 
                     gf.Id, gf.Nome, gf.Ano";

            var parameters = new object[] { ano };

            var listGruposFaturas = await ExecuteSqlRawAsync<GrupoFaturaQueryResult>(sql, parameters);

            return listGruposFaturas;
        }
    }
}
