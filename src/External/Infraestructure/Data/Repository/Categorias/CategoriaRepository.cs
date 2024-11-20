using Application.Constantes;
using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Categorias
{
    public class CategoriaRepository(IServiceProvider service)
        : RepositoryBase<Categoria, FinanceDbContext>(service),
            ICategoriaRepository
    {
        private readonly IServiceProvider _service = service;

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

        public bool IdentificarCategoriaParaAcaoAsync(Guid codeCategoria)
        {
            var categoriaIds = GetCods.CategoriaCod;

            var naoEhAlteravel =
                codeCategoria == categoriaIds.CodAluguel
                || codeCategoria == categoriaIds.CodCondominio
                || codeCategoria == categoriaIds.CodContaDeLuz
                || codeCategoria == categoriaIds.CodAlmoco;

            return naoEhAlteravel;
        }
    }
}
