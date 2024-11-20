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
        private readonly string[] CategoriasArray =
        [
            "Almoço/Janta",
            "Aluguel",
            "Condomínio",
            "Conta de Luz"
        ];

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
                || codeCategoria == categoriaIds.CodAlmoco;

            return naoEhAlteravel;
        }

        public async Task<CategoriaCodsDto> GetCategoriaCodesAsync()
        {
            var categorias = await Get()
                .Where(c => CategoriasArray.Contains(c.Descricao))
                .ToListAsync();

            var idAlmoco = categorias.FirstOrDefault(c => c.Descricao == "Almoço/Janta");
            var idAluguel = categorias.FirstOrDefault(c => c.Descricao == "Aluguel");
            var idCondominio = categorias.FirstOrDefault(c => c.Descricao == "Condomínio");
            var idContaDeLuz = categorias.FirstOrDefault(c => c.Descricao == "Conta de Luz");

            return new CategoriaCodsDto
            {
                CodAluguel = idAluguel.Code,
                CodCondominio = idCondominio.Code,
                CodContaDeLuz = idContaDeLuz.Code,
                CodAlmoco = idAlmoco.Code,
            };
        }
    }
}
