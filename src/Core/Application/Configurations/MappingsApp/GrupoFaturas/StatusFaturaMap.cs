using Application.Commands.Dtos;
using Domain.Dtos;
using Domain.Models.GrupoFaturas;

namespace Application.Configurations.MappingsApp
{
    public static class StatusFaturaMap
    {
        public static StatusFaturaDto MapToDTO(this StatusFatura statusfatura)
        {
            return new StatusFaturaDto
            {
                FaturaNome = statusfatura.FaturaNome,
                Estado = statusfatura.Estado,
                Code = statusfatura.Code
            };
        }

        public static StatusFatura MapToEntity(this StatusFaturaCommandDto statusfaturaDto)
        {
            return new StatusFatura
            {
                FaturaNome = statusfaturaDto.FaturaNome,
                Estado = statusfaturaDto.Estado,
            };
        }

        public static void MapUpdateEntity(
            this StatusFatura statusfatura,
            StatusFaturaCommandDto statusfaturaDto
        )
        {
            statusfatura.FaturaNome = statusfaturaDto.FaturaNome;
            statusfatura.Estado = statusfaturaDto.Estado;
        }
    }
}
