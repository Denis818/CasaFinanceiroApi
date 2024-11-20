using Application.Constantes;
using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories.Membros;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Repository.Membros
{
    public class MembroRepository(IServiceProvider service)
        : RepositoryBase<Membro, FinanceDbContext>(service),
            IMembroRepository
    {
        private readonly IServiceProvider _service = service;

        public async Task<Membro> ExisteAsync(string nome) =>
            await Get(d => d.Nome == nome).FirstOrDefaultAsync();

        public bool ValidaMembroParaAcao(Guid codeMembro)
        {
            var membroI = GetCods.MembroCod;

            var ehAlteravel =
                codeMembro == membroI.CodJhon
                || codeMembro == membroI.CodPeu
                || codeMembro == membroI.CodLaila;

            return ehAlteravel;
        }
    }
}
