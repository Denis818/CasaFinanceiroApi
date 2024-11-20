using Domain.Dtos;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Repositories.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Constantes
{
    public class GetCods
    {
        public static CategoriaCodsDto CategoriaCod { get; private set; }
        public static MembroIdDto MembroCod { get; private set; }

        private static readonly string[] MembrosArray = ["Jhon", "Peu", "Laila"];

        private static readonly string[] CategoriasArray =
        [
            "Almoço/Janta",
            "Aluguel",
            "Condomínio",
            "Conta de Luz"
        ];

        public static async Task GetMembersIds(IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();

            var memberRepo = scope.ServiceProvider.GetRequiredService<IMembroRepository>();

            var membros = await memberRepo.Get(m => MembrosArray.Contains(m.Nome)).ToListAsync();

            Guid? codeJhon = membros.FirstOrDefault(c => c.Nome.StartsWith("Jhon"))?.Code;
            Guid? codePeu = membros.FirstOrDefault(c => c.Nome.StartsWith("Peu"))?.Code;
            Guid? codeLaila = membros.FirstOrDefault(c => c.Nome.StartsWith("Laila"))?.Code;

            MembroCod = new MembroIdDto
            {
                CodJhon = codeJhon ??= new Guid("00000000-0000-0000-0000-000000000000"),
                CodPeu = codePeu ??= new Guid("00000000-0000-0000-0000-000000000000"),
                CodLaila = codeLaila ??= new Guid("00000000-0000-0000-0000-000000000000")
            };
        }

        public static async Task GetCategoriaCodesAsync(IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var categoriaRepo = scope.ServiceProvider.GetRequiredService<ICategoriaRepository>();

            var categorias = await categoriaRepo
                .Get()
                .Where(c => CategoriasArray.Contains(c.Descricao))
                .ToListAsync();

            var idAlmoco = categorias.FirstOrDefault(c => c.Descricao == "Almoço/Janta");
            var idAluguel = categorias.FirstOrDefault(c => c.Descricao == "Aluguel");
            var idCondominio = categorias.FirstOrDefault(c => c.Descricao == "Condomínio");
            var idContaDeLuz = categorias.FirstOrDefault(c => c.Descricao == "Conta de Luz");

            CategoriaCod = new CategoriaCodsDto
            {
                CodAluguel = idAluguel.Code,
                CodCondominio = idCondominio.Code,
                CodContaDeLuz = idContaDeLuz.Code,
                CodAlmoco = idAlmoco.Code,
            };
        }
    }
}
