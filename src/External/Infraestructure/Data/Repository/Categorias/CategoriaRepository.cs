using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Categorias
{
    public class CategoriaRepository
        : RepositoryBase<Categoria, FinanceDbContext>,
            ICategoriaRepository
    {
        private static readonly string[] CategoriasArray =
        [
            "Almoço/Janta",
            "Aluguel",
            "Condomínio",
            "Conta de Luz"
        ];

        public CategoriaRepository(IServiceProvider service)
            : base(service)
        {
        }

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

        public Lazy<CategoriaCodsDto> GetCategoriaCods()
        {
            return new Lazy<CategoriaCodsDto>(() =>
            {
                var categorias = Get()
                    .Where(c => CategoriasArray.Contains(c.Descricao))
                    .ToListAsync()
                    .Result;

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
            });
        }
    }
}
