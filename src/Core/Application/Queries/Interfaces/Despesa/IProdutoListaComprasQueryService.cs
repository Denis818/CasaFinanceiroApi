using Application.Queries.Dtos;

namespace Application.Queries.Interfaces.Despesa
{
    public interface IProdutoListaComprasQueryService
    {
        Task<IEnumerable<ProdutoListaComprasQueryDto>> GetAllAsync();
        Task<byte[]> ExportaPdfListaDeComprasAsync();

    }
}