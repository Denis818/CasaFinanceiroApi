using Application.Queries.Dtos;

namespace Application.Queries.Interfaces.ListaCompras
{
    public interface IProdutoListaComprasQueryService
    {
        Task<IEnumerable<ProdutoListaComprasQueryDto>> GetAllAsync();
        Task<byte[]> ExportaPdfListaDeComprasAsync();

    }
}