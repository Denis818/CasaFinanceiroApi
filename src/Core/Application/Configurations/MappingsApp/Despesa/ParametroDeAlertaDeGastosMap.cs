using Application.Commands.Dtos;
using Application.Queries.Dtos;
using Domain.Models.Despesas;

namespace Application.Configurations.MappingsApp
{
    public static class ParametroDeAlertaDeGastosMap
    {
        public static ParametroDeAlertaDeGastosQueryDto MapToDTO(
            this ParametroDeAlertaDeGastos parametroDeAlertaDeGastos
        )
        {
            return new ParametroDeAlertaDeGastosQueryDto
            {
                TipoMetrica = parametroDeAlertaDeGastos.TipoMetrica,
                LimiteVermelho = parametroDeAlertaDeGastos.LimiteVermelho,
                Code = parametroDeAlertaDeGastos.Code,
                LimiteAmarelo = parametroDeAlertaDeGastos.LimiteAmarelo,
            };
        }

        public static ParametroDeAlertaDeGastos MapToEntity(
            this ParametroDeAlertaDeGastosCommandDto parametroDeAlertaDeGastosDto
        )
        {
            return new ParametroDeAlertaDeGastos
            {
                TipoMetrica = parametroDeAlertaDeGastosDto.TipoMetrica,
                LimiteVermelho = parametroDeAlertaDeGastosDto.LimiteVermelho,
                LimiteAmarelo = parametroDeAlertaDeGastosDto.LimiteAmarelo,
            };
        }

        public static void MapUpdateEntity(
            this ParametroDeAlertaDeGastos parametroDeAlertaDeGastos,
            ParametroDeAlertaDeGastosCommandDto parametroDeAlertaDeGastosDto
        )
        {
            parametroDeAlertaDeGastos.TipoMetrica = parametroDeAlertaDeGastosDto.TipoMetrica;
            parametroDeAlertaDeGastos.LimiteVermelho = parametroDeAlertaDeGastosDto.LimiteVermelho;
            parametroDeAlertaDeGastos.LimiteAmarelo = parametroDeAlertaDeGastosDto.LimiteAmarelo;
        }
    }
}
