using Application.Commands.Dtos;

namespace Application.Commands.Interfaces
{
    public interface IDespesaCommandService
    {
        Task<bool> DeleteAsync(Guid code);
        Task<bool> InsertAsync(DespesaCommandDto despesaDto);
        Task<bool> InsertRangeAsync(IAsyncEnumerable<DespesaCommandDto> listDespesasDto);
        Task<bool> UpdateAsync(Guid code, DespesaCommandDto despesaDto);
    }
}