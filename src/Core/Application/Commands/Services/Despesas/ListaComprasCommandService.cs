using Application.Commands.Dtos.Despesa;
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
    public class ListaComprasCommandService(IServiceProvider service)
        : BaseCommandService<ListaCompras, ListaComprasCommandDto, IListaComprasRepository>(service), IListaComprasCommandService
    {
        protected override ListaCompras MapToEntity(ListaComprasCommandDto entity) =>
            entity.MapToEntity();

        public async Task<bool> InsertAsync(ListaComprasCommandDto listaComprasCommandDto)
        {
            if (Validator(listaComprasCommandDto))
                return false;

            var itemExistente = await _repository.Get().FirstOrDefaultAsync(item => item.Item == listaComprasCommandDto.Item);

            if (itemExistente != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "O Item", listaComprasCommandDto.Item)
                );

                return false;
            }

            var listaCompras = listaComprasCommandDto.MapToEntity();

            await _repository.InsertAsync(listaCompras);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );

                return false;
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid code, ListaComprasCommandDto ListaComprasCommandDto)
        {
            if (ListaComprasCommandDto is null)
            {
                Notificar(
                     EnumTipoNotificacao.NotFount,
                    "Nulo não é valido"
                 );
                return false;

            }
            if (Validator(ListaComprasCommandDto))
                return false;

            var listaCompras = await _repository.GetByCodigoAsync(code);

            if (listaCompras is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "O Item")
                );

                return false;
            }

            listaCompras.MapUpdateEntity(ListaComprasCommandDto);

            _repository.Update(listaCompras);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(Guid code)
        {
            var listaCompras = await _repository.GetByCodigoAsync(code);

            if (listaCompras == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "O Item")
                );

                return false;
            }

            _repository.Delete(listaCompras);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Deletar")
                );

                return false;
            }

            return true;
        }
    }
}
