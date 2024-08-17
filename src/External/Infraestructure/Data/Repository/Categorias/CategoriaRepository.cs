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
        public async Task<Categoria> ExisteAsync(int id, string nome)
        {
            if (nome != null)
            {
                return await Get(c => c.Descricao == nome).FirstOrDefaultAsync();
            }
            else
            {
                return await Get(c => c.Id == id).FirstOrDefaultAsync();
            }
        }

        public bool IdentificarCategoriaParaAcao(int idCategoria)
        {
            var categoriaIds = GetCategoriaIds();

            var ehAlteravel =
                idCategoria == categoriaIds.CodAluguel
                || idCategoria == categoriaIds.CodCondominio
                || idCategoria == categoriaIds.CodContaDeLuz
                || idCategoria == categoriaIds.CodAlmoco
                || idCategoria == categoriaIds.CodInternet;

            return ehAlteravel;
        }

        public CategoriaIdsDto GetCategoriaIds()
        {
            var categ = Get();

            int idAlmoco = categ.FirstOrDefault(c => c.Descricao == "Almoço/Janta").Id;
            int idAluguel = categ.FirstOrDefault(c => c.Descricao == "Aluguel").Id;
            int idCondominio = categ.FirstOrDefault(c => c.Descricao == "Condomínio").Id;
            int idContaDeLuz = categ.FirstOrDefault(c => c.Descricao == "Conta de Luz").Id;
            int idInternet = categ.FirstOrDefault(c => c.Descricao == "Internet").Id;

            return new CategoriaIdsDto
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
