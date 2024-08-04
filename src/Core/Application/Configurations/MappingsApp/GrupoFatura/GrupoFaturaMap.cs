using Application.Commands.Dtos;
using Domain.Models.Despesas;

namespace Application.Configurations.MappingsApp
{
    public static class GrupoFaturaMap
    {
        public static GrupoFatura MapToEntity(this GrupoFaturaCommandDto grupoFaturaDto)
        {
            return new GrupoFatura { Nome = grupoFaturaDto.Nome, Ano = grupoFaturaDto.Ano };
        }
        public static void MapUpdateEntity(this GrupoFatura grupoFatura, GrupoFaturaCommandDto grupoFaturaDto)
        {
            grupoFatura.Nome = grupoFaturaDto.Nome;
            grupoFatura.Ano = grupoFaturaDto.Ano;
        }

    }
}
