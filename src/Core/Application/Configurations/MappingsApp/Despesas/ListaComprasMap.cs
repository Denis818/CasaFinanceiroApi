using Application.Commands.Dtos.Despesa;
using Domain.Dtos.Despesas;
using Domain.Models.Despesas;

namespace Application.Configurations.MappingsApp
{
    public static class ListaComprasMap
    {
        public static ListaComprasQueryDto MapToDTO(this ListaCompras listaCompras)
        {
            return new ListaComprasQueryDto { Item = listaCompras.Item, Code = listaCompras.Code };
        }

        public static ListaCompras MapToEntity(this ListaComprasCommandDto listaComprasDto)
        {
            return new ListaCompras { Item = listaComprasDto.Item, };
        }

        public static void MapUpdateEntity(
            this ListaCompras listaCompras,
            ListaComprasCommandDto listaComprasDto
        )
        {
            listaCompras.Item = listaComprasDto.Item;
        }
    }
}
