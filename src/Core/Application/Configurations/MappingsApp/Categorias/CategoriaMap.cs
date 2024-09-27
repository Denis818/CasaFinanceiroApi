using Application.Commands.Dtos;
using Domain.Dtos.Categoria;
using Domain.Models.Categorias;

namespace Application.Configurations.MappingsApp
{
    public static class CategoriaMap
    {
        public static CategoriaDto MapToDTO(this Categoria categoria)
        {
            return new CategoriaDto
            {
                Descricao = categoria.Descricao,
                Code = categoria.Code
            };
        }

        public static Categoria MapToEntity(this CategoriaCommandDto categoriaDto)
        {
            return new Categoria { Descricao = categoriaDto.Descricao };
        }

        public static void MapUpdateEntity(
            this Categoria categoria,
            CategoriaCommandDto categoriaDto
        )
        {
            categoria.Descricao = categoriaDto.Descricao;
        }
    }
}
