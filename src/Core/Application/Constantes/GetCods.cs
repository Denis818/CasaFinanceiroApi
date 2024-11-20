using Domain.Dtos;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Repositories.Membros;
using Domain.Models.Categorias;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Constantes
{
    public class GetCods
    {
        public static List<Membro> Membros;
        public static List<Categoria> Categorias;

        private static readonly string[] MembrosArray = ["Jhon", "Peu", "Laila"];

        private static readonly string[] CategoriasArray =
        [
            "Almoço/Janta",
            "Aluguel",
            "Condomínio",
            "Conta de Luz"
        ];

        public static async Task<MembroIdDto> GetMembersIds(IServiceProvider service)
        {
            var memberRepo = service.GetRequiredService<IMembroRepository>();

            Membros ??= await memberRepo.Get(m => MembrosArray.Contains(m.Nome)).ToListAsync();

            Guid? codeJhon = Membros.FirstOrDefault(c => c.Nome.StartsWith("Jhon"))?.Code;
            Guid? codePeu = Membros.FirstOrDefault(c => c.Nome.StartsWith("Peu"))?.Code;
            Guid? codeLaila = Membros.FirstOrDefault(c => c.Nome.StartsWith("Laila"))?.Code;

            return new MembroIdDto
            {
                CodJhon = codeJhon ??= new Guid("00000000-0000-0000-0000-000000000000"),
                CodPeu = codePeu ??= new Guid("00000000-0000-0000-0000-000000000000"),
                CodLaila = codeLaila ??= new Guid("00000000-0000-0000-0000-000000000000")
            };
        }

        public static async Task<CategoriaCodsDto> GetCategoriaCodesAsync(IServiceProvider service)
        {
            var categoriaRepo = service.GetRequiredService<ICategoriaRepository>();

            Categorias ??= await categoriaRepo
                .Get()
                .Where(c => CategoriasArray.Contains(c.Descricao))
                .ToListAsync();

            var idAlmoco = Categorias.FirstOrDefault(c => c.Descricao == "Almoço/Janta");
            var idAluguel = Categorias.FirstOrDefault(c => c.Descricao == "Aluguel");
            var idCondominio = Categorias.FirstOrDefault(c => c.Descricao == "Condomínio");
            var idContaDeLuz = Categorias.FirstOrDefault(c => c.Descricao == "Conta de Luz");

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
