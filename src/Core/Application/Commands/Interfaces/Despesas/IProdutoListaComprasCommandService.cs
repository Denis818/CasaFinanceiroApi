using Application.Commands.Dtos.Despesa;

namespace Application.Commands.Interfaces
{
    public interface IProdutoListaComprasCommandService
    {
        Task<bool> DeleteAsync(Guid code);
        Task<bool> InsertAsync(ProdutoListaComprasCommandDto ProdutoListaComprasCommandDto);
        Task<bool> UpdateAsync(Guid code, ProdutoListaComprasCommandDto ProdutoListaComprasCommandDto);
    }
}