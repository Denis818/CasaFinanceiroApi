using Application.Commands.Dtos.Despesa;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.ListaCompras;
using Domain.Models.ListaCompras;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Services.ListaCompras
{
    public class ProdutoListaComprasCommandService(IServiceProvider service)
        : BaseCommandService<ProdutoListaCompras, ProdutoListaComprasCommandDto, IProdutoListaComprasRepository>(service), IProdutoListaComprasCommandService
    {
        protected override ProdutoListaCompras MapToEntity(ProdutoListaComprasCommandDto entity) =>
            entity.MapToEntity();

        public async Task<bool> InsertAsync(ProdutoListaComprasCommandDto ProdutoListaComprasCommandDto)
        {
            if (Validator(ProdutoListaComprasCommandDto))
                return false;

            var itemExistente = await _repository.Get().FirstOrDefaultAsync(item => item.Item == ProdutoListaComprasCommandDto.Item);

            if (itemExistente != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "O Item", ProdutoListaComprasCommandDto.Item)
                );

                return false;
            }

            var ProdutoListaCompras = ProdutoListaComprasCommandDto.MapToEntity();

            await _repository.InsertAsync(ProdutoListaCompras);

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

        public async Task<bool> UpdateAsync(Guid code, ProdutoListaComprasCommandDto ProdutoListaComprasCommandDto)
        {
            if (ProdutoListaComprasCommandDto is null)
            {
                Notificar(
                     EnumTipoNotificacao.NotFount,
                    "Nulo não é valido"
                 );
                return false;

            }
            if (Validator(ProdutoListaComprasCommandDto))
                return false;

            var ProdutoListaCompras = await _repository.GetByCodigoAsync(code);

            if (ProdutoListaCompras is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "O Item")
                );

                return false;
            }

            ProdutoListaCompras.MapUpdateEntity(ProdutoListaComprasCommandDto);

            _repository.Update(ProdutoListaCompras);

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
            var ProdutoListaCompras = await _repository.GetByCodigoAsync(code);

            if (ProdutoListaCompras == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "O Item")
                );

                return false;
            }

            _repository.Delete(ProdutoListaCompras);

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
