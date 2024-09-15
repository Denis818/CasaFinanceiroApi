using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos;
using Domain.Dtos.QueryResults;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Categorias
{
    public class CategoriaRepository(IServiceProvider service)
        : RepositoryBase<Categoria, FinanceDbContext>(service),
            ICategoriaRepository
    {
        public async Task<Categoria> ExisteAsync(Guid? code = null, string nome = "")
        {
            if (nome != null)
            {
                return await Get(c => c.Descricao == nome).FirstOrDefaultAsync();
            }
            else
            {
                return await Get(c => c.Code == code).FirstOrDefaultAsync();
            }
        }

        public async Task<bool> IdentificarCategoriaParaAcaoAsync(Guid codeCategoria)
        {
            var categoriaIds = await GetCategoriaCodesAsync();

            var naoEhAlteravel =
                codeCategoria == categoriaIds.CodAluguel
                || codeCategoria == categoriaIds.CodCondominio
                || codeCategoria == categoriaIds.CodContaDeLuz
                || codeCategoria == categoriaIds.CodAlmoco
                || codeCategoria == categoriaIds.CodInternet;

            return naoEhAlteravel;
        }

        public async Task<CategoriaCodsDto> GetCategoriaCodesAsync()
        {
            var categ = Get();

            var idAlmoco = await categ.FirstOrDefaultAsync(c => c.Descricao == "Almoço/Janta");
            var idAluguel = await categ.FirstOrDefaultAsync(c => c.Descricao == "Aluguel");
            var idCondominio = await categ.FirstOrDefaultAsync(c => c.Descricao == "Condomínio");
            var idContaDeLuz = await categ.FirstOrDefaultAsync(c => c.Descricao == "Conta de Luz");
            var idInternet = await categ.FirstOrDefaultAsync(c => c.Descricao == "Internet");

            return new CategoriaCodsDto
            {
                CodAluguel = idAluguel.Code,
                CodCondominio = idCondominio.Code,
                CodContaDeLuz = idContaDeLuz.Code,
                CodAlmoco = idAlmoco.Code,
                CodInternet = idInternet.Code
            };
        }

        public async Task<IEnumerable<CategoriaQueryResult>> GetAll(Guid grupoCode)
        {
            var sql =
                @"
                 SELECT 
                     c.Code,
                     c.Descricao,
                     COALESCE(SUM(d.Total), 0) AS Total,
                     COUNT(d.Id) AS QuantidadeDeItens 
                 FROM 
                     Categorias c
                 LEFT JOIN 
                     Despesas d ON d.CategoriaId = c.Id AND d.GrupoFaturaCode = {0}
                 GROUP BY 
                     c.Code, c.Descricao
                 ORDER BY
                     c.Descricao";

            var parameters = new object[] { grupoCode };

            var listAgrupada = await ExecuteSqlRawAsync<CategoriaQueryResult>(
                sql,
                parameters
            );

            return listAgrupada;
        }
    }
}
