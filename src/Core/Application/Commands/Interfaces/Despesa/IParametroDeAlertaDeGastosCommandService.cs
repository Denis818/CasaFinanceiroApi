using Application.Commands.Dtos;

namespace Application.Commands.Interfaces
{
    public interface IParametroDeAlertaDeGastosCommandService
    {
        Task<bool> Update(List<ParametroDeAlertaDeGastosCommandDto> metricas);
    }
}
