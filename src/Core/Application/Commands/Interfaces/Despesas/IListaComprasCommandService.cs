using Application.Commands.Dtos.Despesa;

namespace Application.Commands.Interfaces
{
    public interface IListaComprasCommandService
    {
        Task<bool> DeleteAsync(Guid code);
        Task<bool> InsertAsync(ListaComprasCommandDto listaComprasCommandDto);
        Task<bool> UpdateAsync(Guid code, ListaComprasCommandDto ListaComprasCommandDto);
    }
}