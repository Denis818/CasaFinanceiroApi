using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos;
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

            var ehAlteravel =
                   codeCategoria == categoriaIds.CodAluguel
                || codeCategoria == categoriaIds.CodCondominio
                || codeCategoria == categoriaIds.CodContaDeLuz
                || codeCategoria == categoriaIds.CodAlmoco
                || codeCategoria == categoriaIds.CodInternet;

            return ehAlteravel;
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
    }
}
