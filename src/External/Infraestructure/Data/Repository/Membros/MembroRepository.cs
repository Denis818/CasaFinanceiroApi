using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos;
using Domain.Interfaces.Repositories.Membros;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Repository.Membros
{
    public class MembroRepository(IServiceProvider service) : RepositoryBase<Membro, FinanceDbContext>(service), IMembroRepository
    {
        private static readonly string[] MembrosArray = ["Jhon", "Peu", "Laila"];

        public async Task<Membro> ExisteAsync(string nome) =>
            await Get(d => d.Nome == nome).FirstOrDefaultAsync();

        public Lazy<MembroIdDto> GetMembroCods()
        {
            return new Lazy<MembroIdDto>(() =>
            {
                var membros = Get(m => MembrosArray.Contains(m.Nome)).ToListAsync().Result;

                Guid? codeJhon = membros.FirstOrDefault(c => c.Nome.StartsWith("Jhon"))?.Code;
                Guid? codePeu = membros.FirstOrDefault(c => c.Nome.StartsWith("Peu"))?.Code;
                Guid? codeLaila = membros.FirstOrDefault(c => c.Nome.StartsWith("Laila"))?.Code;

                return new MembroIdDto
                {
                    CodJhon = codeJhon ??= new Guid("00000000-0000-0000-0000-000000000000"),
                    CodPeu = codePeu ??= new Guid("00000000-0000-0000-0000-000000000000"),
                    CodLaila = codeLaila ??= new Guid("00000000-0000-0000-0000-000000000000")
                };
            });
        }
    }
}
