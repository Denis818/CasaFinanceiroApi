using Application.Commands.Dtos;

namespace Application.Commands.Interfaces
{
    public interface IDespesaCommandService
    {
        Task<bool> DeleteAsync(Guid code);
        Task InsertAsync(DespesaCommandDto despesaDto);
        Task InsertRangeAsync(IAsyncEnumerable<DespesaCommandDto> listDespesasDto);
        Task UpdateAsync(Guid code, DespesaCommandDto despesaDto);
    }
}