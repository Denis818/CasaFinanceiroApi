using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos;
using Domain.Interfaces.Repositories;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Repository.Membros
{
    public class MembroRepository(IServiceProvider service)
        : RepositoryBase<Membro, FinanceDbContext>(service),
            IMembroRepository
    {
        public async Task<Membro> ExisteAsync(string nome) =>
            await Get(d => d.Nome == nome).FirstOrDefaultAsync();

        public bool ValidaMembroParaAcao(int idMembro)
        {
            var membroI = GetMembersIds();

            var ehAlteravel =
                idMembro == membroI.CodJhon
                || idMembro == membroI.CodPeu
                || idMembro == membroI.CodLaila;

            return ehAlteravel;
        }

        public MembroIdDto GetMembersIds()
        {
            var membros = Get();

            int? idJhon = membros.FirstOrDefault(c => c.Nome.StartsWith("Jhon"))?.Id;
            int? idPeu = membros.FirstOrDefault(c => c.Nome.StartsWith("Peu"))?.Id;
            int? idLaila = membros.FirstOrDefault(c => c.Nome.StartsWith("Laila"))?.Id;

            return new MembroIdDto
            {
                CodJhon = idJhon ?? 0,
                CodPeu = idPeu ?? 0,
                CodLaila = idLaila ?? 0
            };
        }
    }
}
