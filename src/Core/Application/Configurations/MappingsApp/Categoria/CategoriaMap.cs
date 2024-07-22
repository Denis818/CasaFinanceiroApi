using Application.Commands.Dtos;
using Domain.Models.Categorias;

namespace Application.Configurations.MappingsApp
{
    public static class CategoriaMap
    {
        public static Categoria MapToEntity(this CategoriaCommandDto categoriaDto)
        {
            return new Categoria { Descricao = categoriaDto.Descricao };
        }
        public static void MapUpdateEntity(this Categoria categoria, CategoriaCommandDto categoriaDto)
        {
            categoria.Descricao = categoriaDto.Descricao;
        }

    }
}
