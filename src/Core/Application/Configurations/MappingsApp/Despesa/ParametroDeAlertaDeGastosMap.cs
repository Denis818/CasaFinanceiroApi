using Application.Commands.Dtos;
using Domain.Models.Despesas;

namespace Application.Configurations.MappingsApp
{
    public static class ParametroDeAlertaDeGastosMap
    {
        public static ParametroDeAlertaDeGastos MapToEntity(this ParametroDeAlertaDeGastosCommandDto parametroDeAlertaDeGastosDto)
        {
            return new ParametroDeAlertaDeGastos
            {
                TipoMetrica = parametroDeAlertaDeGastosDto.TipoMetrica,
                LimiteVermelho = parametroDeAlertaDeGastosDto.LimiteVermelho,
                LimiteAmarelo = parametroDeAlertaDeGastosDto.LimiteAmarelo,

            };
        }
        public static void MapUpdateEntity(this ParametroDeAlertaDeGastos parametroDeAlertaDeGastos, ParametroDeAlertaDeGastosCommandDto parametroDeAlertaDeGastosDto)
        {
            parametroDeAlertaDeGastos.TipoMetrica = parametroDeAlertaDeGastosDto.TipoMetrica;
            parametroDeAlertaDeGastos.LimiteVermelho = parametroDeAlertaDeGastosDto.LimiteVermelho;
            parametroDeAlertaDeGastos.LimiteAmarelo = parametroDeAlertaDeGastosDto.LimiteAmarelo;
        }
    }
}
