using Application.Commands.Dtos;
using Domain.Models.Despesas;

namespace Application.Configurations.MappingsApp
{
    public static class DespesaMap
    {
        public static Despesa MapToEntity(this DespesaCommandDto despesaDto)
        {
            return new Despesa
            {
                GrupoFaturaId = despesaDto.GrupoFaturaId,
                CategoriaId = despesaDto.CategoriaId,
                Item = despesaDto.Item,
                Preco = despesaDto.Preco,
                Quantidade = despesaDto.Quantidade,
                Fornecedor = despesaDto.Fornecedor
            };
        }
        public static void MapUpdateEntity(this Despesa despesa, DespesaCommandDto despesaDto)
        {
            despesa.GrupoFaturaId = despesaDto.GrupoFaturaId;
            despesa.CategoriaId = despesaDto.CategoriaId;
            despesa.Item = despesaDto.Item;
            despesa.Preco = despesaDto.Preco;
            despesa.Quantidade = despesaDto.Quantidade;
            despesa.Fornecedor = despesaDto.Fornecedor;
        }

    }
}
