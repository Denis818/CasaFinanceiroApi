using Domain.Dtos.Categorias;
using Domain.Dtos.Despesas.Criacao;
using Domain.Dtos.Membros;
using Domain.Models.Categorias;
using Domain.Models.Despesas;
using Domain.Models.Membros;

namespace Application.Services.Mappings
{
    public static class MapperEntites
    {
        public static Categoria MapToEntity(this CategoriaDto categoriaDto)
        {
            return new Categoria { Descricao = categoriaDto.Descricao };
        }

        public static Membro MapToEntity(this MembroDto membroDto)
        {
            return new Membro { Nome = membroDto.Nome, Telefone = membroDto.Telefone };
        }

        public static GrupoFatura MapToEntity(this GrupoFaturaDto grupoFaturaDto)
        {
            return new GrupoFatura { Nome = grupoFaturaDto.Nome };
        }

        public static Despesa MapToEntity(this DespesaDto despesaDto)
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

        public static void MapUpdateEntity(this Categoria categoria, CategoriaDto categoriaDto)
        {
            categoria.Descricao = categoriaDto.Descricao;
        }

        public static void MapUpdateEntity(this Membro membro, MembroDto membroDto)
        {
            membro.Nome = membroDto.Nome;
            membro.Telefone = membroDto.Telefone;
        }

        public static void MapUpdateEntity(this GrupoFatura grupoFatura, GrupoFaturaDto grupoFaturaDto)
        {
            grupoFatura.Nome = grupoFaturaDto.Nome;
        }

        public static void MapUpdateEntity(this Despesa despesa, DespesaDto despesaDto)
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