using Application.Commands.Dtos;
using Domain.Dtos;
using Domain.Models.Despesas;

namespace Application.Configurations.MappingsApp
{
    public static class DespesaMap
    {
        public static DespesaDto MapToDTO(this Despesa despesa)
        {
            return new DespesaDto
            {
                Code = despesa.Code,
                Item = despesa.Item,
                Preco = despesa.Preco,
                Quantidade = despesa.Quantidade,
                Fornecedor = despesa.Fornecedor,
                DataCompra = despesa.DataCompra,
                Total = despesa.Total,
                GrupoFatura = despesa.GrupoFatura?.MapToDTO(),
                Categoria = despesa.Categoria?.MapToDTO()
            };
        }

        public static Despesa MapToEntity(this DespesaCommandDto despesaDto)
        {
            return new Despesa
            {
                Item = despesaDto.Item,
                Preco = despesaDto.Preco,
                Quantidade = despesaDto.Quantidade,
                Fornecedor = despesaDto.Fornecedor,
                GrupoFaturaId = despesaDto.GrupoFaturaId,
                CategoriaId = despesaDto.CategoriaId,
                CategoriaCode = despesaDto.CategoriaCode,
                GrupoFaturaCode = despesaDto.GrupoFaturaCode,
            };
        }

        public static void MapUpdateEntity(this Despesa despesa, DespesaCommandDto despesaDto)
        {
            despesa.Item = despesaDto.Item;
            despesa.Preco = despesaDto.Preco;
            despesa.Quantidade = despesaDto.Quantidade;
            despesa.Fornecedor = despesaDto.Fornecedor;
            despesa.GrupoFaturaId = despesaDto.GrupoFaturaId;
            despesa.CategoriaId = despesaDto.CategoriaId;
            despesa.GrupoFaturaCode = despesaDto.GrupoFaturaCode;
            despesa.CategoriaCode = despesaDto.CategoriaCode;
        }
    }
}
