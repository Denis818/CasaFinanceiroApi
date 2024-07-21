using Domain.Dtos.Despesas.Criacao;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Finance.Comandos
{
    public interface IDespesaComandoService
    {
        Task<bool> DeleteAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);
    }
}