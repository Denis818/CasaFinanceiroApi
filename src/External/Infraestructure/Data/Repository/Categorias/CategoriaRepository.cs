using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos;
using Domain.Interfaces.Repositories.Categorias;
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
    }
}
