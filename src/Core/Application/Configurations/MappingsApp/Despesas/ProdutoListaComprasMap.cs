using Application.Commands.Dtos.Despesa;
using Application.Queries.Dtos;
using Domain.Models.ListaCompras;

namespace Application.Configurations.MappingsApp
{
    public static class ProdutoListaComprasMap
    {
        public static ProdutoListaComprasQueryDto MapToDTO(this ProdutoListaCompras ProdutoListaCompras)
        {
            return new ProdutoListaComprasQueryDto { Item = ProdutoListaCompras.Item, Code = ProdutoListaCompras.Code };
        }

        public static ProdutoListaCompras MapToEntity(this ProdutoListaComprasCommandDto ProdutoListaComprasDto)
        {
            return new ProdutoListaCompras { Item = ProdutoListaComprasDto.Item, };
        }

        public static void MapUpdateEntity(
            this ProdutoListaCompras ProdutoListaCompras,
            ProdutoListaComprasCommandDto ProdutoListaComprasDto
        )
        {
            ProdutoListaCompras.Item = ProdutoListaComprasDto.Item;
        }
    }
}
