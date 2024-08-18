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

        public bool IdentificarCategoriaParaAcao(Guid codeCategoria)
        {
            var categoriaIds = GetCategoriaCodes();

            var naoEhAlteravel =
                codeCategoria == categoriaIds.CodAluguel
                || codeCategoria == categoriaIds.CodCondominio
                || codeCategoria == categoriaIds.CodContaDeLuz
                || codeCategoria == categoriaIds.CodAlmoco
                || codeCategoria == categoriaIds.CodInternet;

            return naoEhAlteravel;
        }

        public CategoriaCodsDto GetCategoriaCodes()
        {
            var categ = Get();

            Guid idAlmoco = categ.FirstOrDefault(c => c.Descricao == "Almoço/Janta").Code;
            Guid idAluguel = categ.FirstOrDefault(c => c.Descricao == "Aluguel").Code;
            Guid idCondominio = categ.FirstOrDefault(c => c.Descricao == "Condomínio").Code;
            Guid idContaDeLuz = categ.FirstOrDefault(c => c.Descricao == "Conta de Luz").Code;
            Guid idInternet = categ.FirstOrDefault(c => c.Descricao == "Internet").Code;

            return new CategoriaCodsDto
            {
                CodAluguel = idAluguel,
                CodCondominio = idCondominio,
                CodContaDeLuz = idContaDeLuz,
                CodAlmoco = idAlmoco,
                CodInternet = idInternet
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
                     Despesas d ON d.CategoriaCode = c.Code AND d.GrupoFaturaCode = {0}
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
