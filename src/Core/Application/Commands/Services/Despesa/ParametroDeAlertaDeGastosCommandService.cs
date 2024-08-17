using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Services
{
    public class ParametroDeAlertaDeGastosCommandService(IServiceProvider service)
        : BaseCommandService<
            ParametroDeAlertaDeGastos,
            ParametroDeAlertaDeGastosCommandDto,
            IParametroDeAlertaDeGastosRepository
        >(service),
            IParametroDeAlertaDeGastosCommandService
    {
        protected override ParametroDeAlertaDeGastos MapToEntity(
            ParametroDeAlertaDeGastosCommandDto entity
        ) => entity.MapToEntity();

        public async Task<bool> Update(
            List<ParametroDeAlertaDeGastosCommandDto> listParametroDeAlertaDeGastosDto
        )
        {
            foreach (var parametroDeAlertaDeGastosDto in listParametroDeAlertaDeGastosDto)
            {
                if (
                    parametroDeAlertaDeGastosDto.LimiteVermelho
                    < parametroDeAlertaDeGastosDto.LimiteAmarelo
                )
                {
                    Notificar(
                        EnumTipoNotificacao.ClientError,
                        "O limite vermelho não pode ser menor que o limite amarelo."
                    );

                    return false;
                }

                var parametroDeAlertaDeGastos = await _repository
                    .Get(parametroDeAlertaDeGastos =>
                        /*parametroDeAlertaDeGastos.Id == parametroDeAlertaDeGastosDto.Id &&*/
                        parametroDeAlertaDeGastos.TipoMetrica
                        == parametroDeAlertaDeGastosDto.TipoMetrica
                    )
                    .FirstOrDefaultAsync();

                if (parametroDeAlertaDeGastos is null)
                {
                    Notificar(
                        EnumTipoNotificacao.ClientError,
                        "Não foi encontrada uma métrica com esse id."
                    );
                    return false;
                }

                parametroDeAlertaDeGastos.MapUpdateEntity(parametroDeAlertaDeGastosDto);

                _repository.Update(parametroDeAlertaDeGastos);

                if (!await _repository.SaveChangesAsync())
                {
                    Notificar(
                        EnumTipoNotificacao.ServerError,
                        string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                    );

                    return false;
                }
            }

            return true;
        }
    }
}
