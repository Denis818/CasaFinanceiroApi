using Application.Commands.Dtos;
using Domain.Models.Despesas;

namespace Application.Configurations.MappingsApp
{
    public static class GrupoFaturaMap
    {
        public static GrupoFatura MapToEntity(this GrupoFaturaCommandDto grupoFaturaDto)
        {
            return new GrupoFatura { Nome = grupoFaturaDto.Nome };
        }
        public static void MapUpdateEntity(this GrupoFatura grupoFatura, GrupoFaturaCommandDto grupoFaturaDto)
        {
            grupoFatura.Nome = grupoFaturaDto.Nome;
        }

    }
}
