using Application.Commands.Dtos;
using Domain.Models.Despesas;

namespace Application.Commands.Interfaces
{
    public interface IDespesaCommandService
    {
        Task<bool> DeleteAsync(int id);
        Task<Despesa> InsertAsync(DespesaCommandDto despesaDto);
        Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaCommandDto> listDespesasDto);
        Task<Despesa> UpdateAsync(int id, DespesaCommandDto despesaDto);
    }
}